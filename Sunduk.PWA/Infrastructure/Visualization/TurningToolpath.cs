using System;
using System.Collections.Generic;
using System.Linq;
using Sunduk.Geometry;
using Sunduk.Geometry.ContourElements;
using Sunduk.Geometry.ContourElements.Base;
using ArcElement = Sunduk.Geometry.ContourElements.Arc;
using LineElement = Sunduk.Geometry.ContourElements.Line;

namespace Sunduk.PWA.Infrastructure.Visualization
{
    /// <summary>Точка в плоскости XZ в "истинных" координатах: Z — вдоль оси (вправо), R — радиус
    /// (вверх; X программы — диаметр, R = X / 2). Все вычисления траектории идут в (Z, R), чтобы дуги
    /// были настоящими окружностями; в диаметр результат переводит только отрисовка/подписи.</summary>
    public readonly record struct Pt(double Z, double R);

    /// <summary>Одно перемещение инструмента — отрезок или дуга (G0/G1/G2/G3).</summary>
    public sealed class PathMove
    {
        public bool Rapid { get; init; }
        public Pt From { get; init; }
        public Pt To { get; init; }
        /// <summary>Радиус дуги; 0 — прямая.</summary>
        public double Radius { get; init; }
        /// <summary>Направление дуги (в плоскости "Z вправо, X вверх"): CW = G2, CCW = G3.</summary>
        public Direction Direction { get; init; }

        public bool IsArc => Radius > 0;

        /// <summary>Центр дуги. Радиус в G2/G3 R-форме всегда задаёт дугу не больше полуокружности:
        /// центр лежит слева от хорды при CCW и справа при CW.</summary>
        public Pt Center
        {
            get
            {
                var dz = To.Z - From.Z;
                var dr = To.R - From.R;
                var len = Math.Sqrt(dz * dz + dr * dr);
                if (len < 1e-12) return From;
                var h = Math.Sqrt(Math.Max(Radius * Radius - len * len / 4, 0));
                var sign = Direction == Direction.CCW ? 1 : -1;
                return new Pt((From.Z + To.Z) / 2 + sign * h * -dr / len, (From.R + To.R) / 2 + sign * h * dz / len);
            }
        }

        /// <summary>Точки для геометрических запросов (пересечения, габариты): у дуги — ломаная.</summary>
        public IEnumerable<Pt> Sample(int arcSteps = 24)
        {
            yield return From;
            if (!IsArc) { yield return To; yield break; }
            var c = Center;
            var a0 = Math.Atan2(From.R - c.R, From.Z - c.Z);
            var a1 = Math.Atan2(To.R - c.R, To.Z - c.Z);
            var sweep = a1 - a0;
            if (Direction == Direction.CCW) { while (sweep <= 0) sweep += 2 * Math.PI; }
            else { while (sweep >= 0) sweep -= 2 * Math.PI; }
            for (var i = 1; i < arcSteps; i++)
            {
                var a = a0 + sweep * i / arcSteps;
                yield return new Pt(c.Z + Radius * Math.Cos(a), c.R + Radius * Math.Sin(a));
            }
            yield return To;
        }
    }

    /// <summary>Метка скругления/фаски на узле контура — для подписи на превью. Если угол не удалось
    /// скруглить, <see cref="Reason"/> объясняет почему (G-код в этом случае R/C тоже не пишет).</summary>
    public sealed record CornerMark(Pt Node, double Size, Blunt Type, string? Reason)
    {
        public bool Applied => Reason is null;
        public string Label => (Type is Blunt.SimpleChamfer ? "C" : "R") + Math.Round(Size, 3).ToString(System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Желаемая траектория токарного перехода в плоскости XZ — номинальный (чертёжный) контур
    /// со скруглениями/фасками, как он задан в редакторе. От инструмента не зависит: смена
    /// инструмента пересчитывает только G-код (компенсация радиуса пластины), а не этот контур.
    /// Проходы цикла и путь вершины намеренно не рисуются.
    /// </summary>
    public sealed class TurningToolpath
    {
        /// <summary>Номинальный (чертёжный) контур детали с учтёнными скруглениями/фасками.</summary>
        public List<PathMove> Nominal { get; } = new();

        /// <summary>Узлы номинального контура с заданным R/C (применённые и нет).</summary>
        public List<CornerMark> Corners { get; } = new();

        /// <summary>Все характерные точки траектории (для габаритов сцены).</summary>
        public IEnumerable<Pt> AllPoints() => Nominal.SelectMany(m => m.Sample(8));

        /// <summary>Подставляет пропущенные X/Z предыдущими (как в G-коде и превью); контур, у которого
        /// первый узел без координат, считать нельзя.</summary>
        private static List<Element>? Resolve(IReadOnlyList<Element> contour)
        {
            if (contour is null || contour.Count < 2) return null;
            var result = new List<Element>(contour.Count);
            double? px = null, pz = null;
            foreach (var e in contour)
            {
                var x = e.X ?? px;
                var z = e.Z ?? pz;
                if (x is null || z is null) return null;
                result.Add(e switch
                {
                    Point p => new Point(x, z, p.Blunt),
                    LineElement l => new LineElement(x, z, l.Angle, l.Blunt, l.BluntType),
                    ArcElement a => new ArcElement(x, z, a.Radius, a.Direction, a.CenterX, a.CenterZ),
                    _ => e,
                });
                px = x; pz = z;
            }
            return result;
        }

        /// <summary>Строит траекторию по контуру. null — контур неполный (меньше двух узлов с
        /// координатами).</summary>
        public static TurningToolpath? Build(IReadOnlyList<Element> contour)
        {
            var resolved = Resolve(contour);
            if (resolved is null) return null;
            var path = new TurningToolpath();
            path.Nominal.AddRange(Moves(resolved, path.Corners));
            return path;
        }

        // ------------------------------------------------------------------
        // Контур → перемещения (отрезки/дуги с R/C на узлах)
        // ------------------------------------------------------------------


        /// <summary>Узел контура + то, как обрабатывается угол в нём: входная/выходная точки
        /// (совпадают с самим узлом, если скругления нет), и перемещение угла (дуга или фаска).</summary>
        private sealed class Corner
        {
            public Pt In, Out;
            public PathMove? Move;
        }

        private static List<PathMove> Moves(List<Element> nodes, List<CornerMark>? marks = null)
        {
            var n = nodes.Count;
            var pts = nodes.Select(e => new Pt(e.Z!.Value, e.X!.Value / 2)).ToArray();
            bool IsValidArc(int i)
            {
                if (i <= 0 || nodes[i] is not ArcElement a || a.Radius <= 0) return false;
                var dz = pts[i].Z - pts[i - 1].Z;
                var dr = pts[i].R - pts[i - 1].R;
                return a.Radius * 2 >= Math.Sqrt(dz * dz + dr * dr) - 1e-9;
            }

            var corners = new Corner[n];
            for (var i = 0; i < n; i++) corners[i] = new Corner { In = pts[i], Out = pts[i] };

            // мнимый торец перед первым узлом — вертикальный (по X), инструмент идёт от большего диаметра
            for (var i = 0; i < n - 1; i++)
            {
                var (blunt, type) = nodes[i] switch
                {
                    Point p => (p.Blunt, Blunt.Radius),
                    LineElement l => (l.Blunt, l.BluntType),
                    _ => (0d, Blunt.Radius),
                };
                if (blunt <= 0) continue;
                if (IsValidArc(i + 1))
                {
                    marks?.Add(new CornerMark(pts[i], blunt, type, "дальше дуга — R/C не применяется"));
                    continue;
                }
                var size = blunt;
                Pt a;
                if (i == 0)
                {
                    a = new Pt(pts[0].Z, pts[0].R + size); // мнимый торец выше первого узла
                }
                else
                {
                    if (IsValidArc(i)) { marks?.Add(new CornerMark(pts[i], blunt, type, "угол на дуге")); continue; }
                    a = pts[i - 1];
                }
                var b = pts[i + 1];
                var prevTrimmed = i > 0 && corners[i - 1].Move is not null;
                var limitIn = Dist(a, pts[i]) * (prevTrimmed ? 0.5 : 1);
                var limitOut = Dist(pts[i], b) * 0.5;
                var rounded = RoundCorner(a, pts[i], b, size, type, limitIn, limitOut);
                if (rounded is not null) corners[i] = rounded;
                marks?.Add(new CornerMark(pts[i], blunt, type, rounded is null ? "стороны угла на одной прямой" : null));
            }
            // последний узел: следующего перемещения нет, G-код R/C на нём не пишет
            if (marks is not null)
            {
                var (lastBlunt, lastType) = nodes[n - 1] switch
                {
                    Point p => (p.Blunt, Blunt.Radius),
                    LineElement l => (l.Blunt, l.BluntType),
                    _ => (0d, Blunt.Radius),
                };
                if (lastBlunt > 0) marks.Add(new CornerMark(pts[n - 1], lastBlunt, lastType, "последний узел — R/C не применяется"));
            }

            var result = new List<PathMove>();
            for (var i = 0; i < n; i++)
            {
                if (i > 0)
                {
                    var from = corners[i - 1].Out;
                    var to = corners[i].In;
                    if (Dist(from, to) > 1e-9)
                    {
                        result.Add(IsValidArc(i)
                            ? new PathMove { From = from, To = to, Radius = ((ArcElement)nodes[i]).Radius, Direction = ((ArcElement)nodes[i]).Direction }
                            : new PathMove { From = from, To = to });
                    }
                }
                if (corners[i].Move is { } cm) result.Add(cm);
            }
            return result;
        }

        private static double Dist(Pt a, Pt b) => Math.Sqrt((a.Z - b.Z) * (a.Z - b.Z) + (a.R - b.R) * (a.R - b.R));

        private static Corner? RoundCorner(Pt a, Pt p, Pt b, double size, Blunt type, double limitIn, double limitOut)
        {
            var d1 = Unit(a, p);
            var d2 = Unit(p, b);
            var cross = d1.Z * d2.R - d1.R * d2.Z;
            var dot = Math.Clamp(d1.Z * d2.Z + d1.R * d2.R, -1, 1);
            if (Math.Abs(cross) < 1e-9) return null;
            var theta = Math.Acos(dot); // угол поворота направления на стыке
            var limit = Math.Min(limitIn, limitOut);
            double t, radius = 0;
            if (type is Blunt.SimpleChamfer)
            {
                t = Math.Min(size, limit);
            }
            else
            {
                radius = size;
                t = radius * Math.Tan(theta / 2);
                if (t > limit) { t = limit; radius = t / Math.Tan(theta / 2); }
            }
            if (t < 1e-9) return null;
            var tIn = new Pt(p.Z - d1.Z * t, p.R - d1.R * t);
            var tOut = new Pt(p.Z + d2.Z * t, p.R + d2.R * t);
            var move = type is Blunt.SimpleChamfer
                ? new PathMove { From = tIn, To = tOut }
                : new PathMove { From = tIn, To = tOut, Radius = radius, Direction = cross > 0 ? Direction.CCW : Direction.CW };
            return new Corner { In = tIn, Out = tOut, Move = move };
        }

        private static Pt Unit(Pt from, Pt to)
        {
            var len = Dist(from, to);
            return len < 1e-12 ? new Pt(0, 0) : new Pt((to.Z - from.Z) / len, (to.R - from.R) / len);
        }
    }
}
