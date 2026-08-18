using System;
using System.Collections.Generic;
using Sunduk.Geometry.ContourElements;
using Sunduk.Geometry.ContourElements.Base;

namespace Sunduk.Geometry
{
    /// <summary>
    /// Компенсация радиуса пластины для точения по контуру — координаты пересчитываются с
    /// номинальных (чертёжных) на координаты мнимой вершины инструмента. Вектор вершины
    /// (<see cref="ToolNoseVector"/>) — угол описанного вокруг пластины квадрата: вектор 3
    /// (нижний левый, наружное точение) или вектор 2 (верхний левый, внутреннее/расточное),
    /// выбирается параметром <paramref name="noseVector"/>. Вдоль участков, идущих строго по Z
    /// (цилиндр) или строго по X (торец), координата по другой оси не меняется — прямые кромки
    /// пластины у мнимой вершины и так ориентированы вдоль этих направлений.
    /// <br/><br/>
    /// Узлы с явно заданным Blunt/BluntType на стыке прямая-прямая этот модуль НЕ трогает — их
    /// скругление уже делает сам G-код через стандартную автоматическую обработку угла контроллера
    /// (см. <c>GCodeBuilder.Contour</c> и его BluntSuffix, адреса R/C на G1) —
    /// компенсация там сводится к тому, чтобы прибавить радиус пластины к запрограммированному
    /// размеру фаски/скругления. R/C-адрес работает только когда ОБЕ стороны стыка — прямые
    /// (включая мнимый торец перед первым узлом контура); если одна из сторон — дуга, контроллер
    /// так скруглить не умеет, и этот модуль сам материализует скругление отдельной маленькой дугой
    /// (см. <see cref="ArcAnchor.Face"/>/<see cref="ArcAnchor.Cylinder"/> — формула вынесена в
    /// общий <see cref="ArcAnchor"/>, им же пользуется калькулятор произвольной дуги в "Прочее").
    /// Blunt живёт только на Point/Line, не на Arc — поэтому скругление стыка прямая-дуга
    /// представимо только когда прямая идёт ПЕРЕД дугой (Blunt стоит на прямой, дуга — следующий
    /// узел); стык, где дуга идёт первой и скругление нужно на её собственном конце, в текущей
    /// модели контура задать нельзя (не хватает поля на Arc) — ограничение редактора контура, а не
    /// этого модуля.
    /// <br/><br/>
    /// <b>Стык прямая-прямая под углом</b> (один выровненный по оси сосед + наклонная кромка, или
    /// обе кромки наклонные — единая модель для обоих случаев): каждая кромка, включая мнимый
    /// торец перед первым узлом контура, сдвигается вдоль своей собственной нормали
    /// <c>n=(dir.Z,-dir.X)</c> на <c>toolRadius</c>; пересечение двух сдвинутых прямых (см.
    /// <see cref="IntersectLines"/>) — центр окружности (носика), касательной обеим кромкам
    /// одновременно. Вершина инструмента = center+(+R,-R) для вектора 2, center+(-R,-R) для
    /// вектора 3 — один и тот же центр для обоих векторов, разница только в знаке X-компоненты
    /// офсета. При почти параллельных кромках (пересечение сдвинутых прямых не определено) узел
    /// остаётся нескомпенсированным.
    /// <br/><br/>
    /// <b>Стык прямая-дуга под острым (некасательным) углом, без Blunt</b> (торец ИЛИ цилиндр,
    /// наружное И внутреннее точение): сдвиг по оси выровненного соседа через общий
    /// <see cref="ArcAnchor.Sharp"/> — угол касательной дуги в точке стыка + <see cref="GeometryMath.ChamferShifts"/>.
    /// <br/><br/>
    /// <b>Касательный (или близкий к касательному) вход/выход дуги</b> — отдельная простая
    /// (нетригонометрическая) формула <see cref="TangentialOffset"/>: сдвиг по оси соседа на радиус
    /// пластины (торец по X, цилиндр по Z), совпадает с <c>Arc2Component.StartDiamWithShift</c>.
    /// <br/><br/>
    /// <b>Стык дуга-дуга</b> — та же модель мнимой вершины, что для прямой-прямой, но по касательным
    /// обеих дуг в точке стыка (<see cref="CornerVertex"/>); касательный стык дуга-дуга даёт
    /// IntersectLines=null и узел не двигается — там достаточно коррекции радиусов.
    /// <br/><br/>
    /// <b>Явное скругление (Blunt) на стыке прямая-дуга</b> (торец или цилиндр перед дугой) —
    /// вставляется отдельная маленькая дуга сопряжения через <see cref="ArcAnchor.Face"/> /
    /// <see cref="ArcAnchor.Cylinder"/>.
    /// <br/><br/>
    /// <b>Радиус дуги</b> — всегда ± радиус пластины, знак по выпуклости/вогнутости относительно
    /// инструмента (<see cref="IsConvexToTool"/>, выводится из <see cref="Arc.Direction"/> и стороны
    /// материала). Центр дуги при этом не двигается — он используется только как номинальный
    /// ориентир для касательных, не как истинный центр скомпенсированной дуги.
    /// <br/><br/>
    /// <b>Статус сверки.</b> Сверено с эталоном (Arc2Component / NippleComponent): касательный вход
    /// с торца, радиус выпуклой дуги, острый стык торец-дуга. Выведено, но требует подтверждения
    /// числом из CAD: острый стык цилиндр-дуга, касательный вход/выход на цилиндре, стык дуга-дуга,
    /// внутреннее точение (знак — зеркальный, без внешнего эталона). Работает в истинных радиусных
    /// координатах (не диаметр) — конвертация на границе модуля.
    /// </summary>
    public static class ToolTipCompensation
    {
        private const double Tolerance = 1e-6;

        /// <summary>Виртуальный торец перед первым узлом контура — направление движения инструмента
        /// в первый узел вдоль этого торца, т.е. от большего диаметра к первому узлу (убывающий X).</summary>
        private static readonly GDirection VirtualFaceDirection = new(-1, 0);

        /// <summary>
        /// Точка входа: номинальный контур (X — диаметр, как везде в проекте) → контур,
        /// готовый для рендера <c>GCodeBuilder.Contour</c>, с учтённым радиусом
        /// пластины <paramref name="toolRadius"/> на острых стыках и дугах.
        /// <paramref name="noseVector"/> — вектор мнимой вершины инструмента
        /// (<see cref="ToolNoseVector"/>), из него выводится сторона материала (наружное/внутреннее),
        /// определяющая знак поправки.
        /// </summary>
        public static List<Element> Compensate(List<Element> contour, double toolRadius, ToolNoseVector noseVector)
        {
            if (contour.Count == 0 || toolRadius <= 0) return contour;

            var external = noseVector.IsExternal();
            var nodes = ToRadiusNodes(contour);
            var (compensated, finalizedArcs) = CompensateCorners(nodes, toolRadius, external);
            AdjustArcRadii(compensated, toolRadius, external, finalizedArcs);
            return ToDiameterElements(compensated);
        }

        // ------------------------------------------------------------------
        // Внутреннее представление: те же типы узлов (Point/Line/Arc), но X уже переведён в
        // истинный радиус вместо диаметра — вся геометрия ниже считается в этих координатах.
        // ------------------------------------------------------------------

        private static List<Element> ToRadiusNodes(List<Element> contour)
        {
            var result = new List<Element>(contour.Count);
            foreach (var e in contour)
            {
                var r = e.X / 2;
                result.Add(e switch
                {
                    Point p => new Point(r, p.Z, p.Blunt),
                    Line l => new Line(r, l.Z, l.Angle, l.Blunt, l.BluntType),
                    Arc a => new Arc(r, a.Z, a.Radius, a.Direction, a.CenterX / 2, a.CenterZ),
                    _ => e,
                });
            }
            return result;
        }

        private static List<Element> ToDiameterElements(List<Element> nodes)
        {
            var result = new List<Element>(nodes.Count);
            foreach (var e in nodes)
            {
                var d = e.X * 2;
                result.Add(e switch
                {
                    Point p => new Point(d, p.Z, p.Blunt),
                    Line l => new Line(d, l.Z, l.Angle, l.Blunt, l.BluntType),
                    Arc a => new Arc(d, a.Z, a.Radius, a.Direction, a.CenterX * 2, a.CenterZ),
                    _ => e,
                });
            }
            return result;
        }

        // ------------------------------------------------------------------
        // Единый проход по стыкам контура. Строит НОВЫЙ список узлов (стык с явным Blunt на
        // границе с дугой превращается из одного узла в два — сдвинутая точка на торце + отдельная
        // маленькая дуга сопряжения), поэтому не мутирует `nodes` — все TangentInto/TangentOutOf
        // подстановки читают из исходного (немутированного) `nodes` по оригинальным индексам,
        // что корректно: вставка узла в другом месте контура не меняет геометрию соседних стыков.
        // Возвращает также индексы (в РЕЗУЛЬТИРУЮЩЕМ списке) только что вставленных маленьких дуг
        // сопряжения — их радиус уже финальный (Blunt + toolRadius), AdjustArcRadii не должен
        // прибавлять radius пластины к ним ещё раз.
        // ------------------------------------------------------------------

        private static (List<Element> Nodes, HashSet<int> FinalizedArcs) CompensateCorners(List<Element> nodes, double toolRadius, bool external)
        {
            var result = new List<Element>(nodes.Count);
            var finalizedArcs = new HashSet<int>();

            for (var i = 0; i < nodes.Count; i++)
            {
                var curr = nodes[i];
                if (curr.X is null || curr.Z is null) { result.Add(curr); continue; }

                var next = i + 1 < nodes.Count ? nodes[i + 1] : null;
                var hasExplicitBlunt = curr switch
                {
                    Point p => p.Blunt > 0,
                    Line l => l.Blunt > 0,
                    _ => false,
                };
                var incomingIsArc = curr is Arc;
                var outgoingIsArc = next is Arc;

                // Явное скругление на стыке прямая-дуга (торец ИЛИ цилиндр перед дугой) — R/C-адрес
                // контроллера тут не работает (умеет только прямая-прямая, см. класс-докстринг),
                // нужна отдельная маленькая дуга сопряжения, как в NippleComponent.GetBluntedShape
                // (RoundCorner-режим) для торца и Arc2Component.GetBluntedShape для цилиндра.
                if (hasExplicitBlunt && external && !incomingIsArc && outgoingIsArc && next is Arc arcNext)
                {
                    var blunt = curr switch { Point p => p.Blunt, Line l => l.Blunt, _ => 0 };
                    var inDirBlunt = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    if (inDirBlunt is not null && IsFaceDirection(inDirBlunt.Value))
                    {
                        var blunted = ArcAnchor.Face(curr.Z.Value, arcNext.CenterX, arcNext.CenterZ, arcNext.Radius, blunt, toolRadius);
                        if (blunted is not null)
                        {
                            result.Add(curr switch
                            {
                                Point p => new Point(blunted.Value.StartX, curr.Z.Value, 0),
                                Line l => new Line(blunted.Value.StartX, curr.Z.Value, l.Angle, 0, l.BluntType),
                                _ => curr,
                            });
                            finalizedArcs.Add(result.Count);
                            result.Add(new Arc(blunted.Value.EndX, blunted.Value.EndZ, blunt + toolRadius, arcNext.Direction, arcNext.CenterX, arcNext.CenterZ));
                            continue;
                        }
                    }
                    else if (inDirBlunt is not null && IsCylinderDirection(inDirBlunt.Value))
                    {
                        var blunted = ArcAnchor.Cylinder(curr.X.Value, arcNext.CenterX, arcNext.CenterZ, arcNext.Radius, blunt, toolRadius);
                        if (blunted is not null)
                        {
                            result.Add(curr switch
                            {
                                Point p => new Point(curr.X.Value, blunted.Value.TangentZ, 0),
                                Line l => new Line(curr.X.Value, blunted.Value.TangentZ, l.Angle, 0, l.BluntType),
                                _ => curr,
                            });
                            finalizedArcs.Add(result.Count);
                            result.Add(new Arc(blunted.Value.HandoffX, blunted.Value.HandoffZ, blunt + toolRadius, arcNext.Direction, arcNext.CenterX, arcNext.CenterZ));
                            continue;
                        }
                    }
                }

                if (hasExplicitBlunt) { result.Add(curr); continue; } // line-line blunt — обработает BluntSuffix

                if (next is null) { result.Add(curr); continue; } // последний узел — стыка после него нет

                var outgoingAligned = IsAxisAligned(curr, next);
                var incomingAligned = i == 0 || IsAxisAligned(nodes[i - 1], curr);
                if (incomingAligned && outgoingAligned) { result.Add(curr); continue; } // прямой выровненный угол — поправка не нужна

                if (!incomingIsArc && !outgoingIsArc)
                {
                    var inDir = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    var outDir = TangentOutOf(nodes, i);
                    if (inDir is null || outDir is null) { result.Add(curr); continue; }

                    // Единая модель мнимой вершины для стыка двух прямых (см. класс-докстринг):
                    // каждая кромка сдвигается вдоль своей нормали n=(dir.Z,-dir.X) на toolRadius,
                    // пересечение двух сдвинутых прямых — центр окружности, касательной обеим
                    // кромкам; вершина = center+(+R,-R) для вектора 2, center+(-R,-R) для вектора 3.
                    var compensated = CornerVertex(new GPoint(curr.X.Value, curr.Z.Value), inDir.Value, outDir.Value, toolRadius, external);
                    if (compensated is not null)
                    {
                        result.Add(CompensateNode(curr, compensated.Value.X, compensated.Value.Z));
                        continue;
                    }
                    // Кромки почти параллельны — пересечение смещённых прямых не определено,
                    // оставляем узел нескомпенсированным, не подставляем неустойчивую формулу.
                }

                // Ровно одна сторона стыка — дуга: прямой сосед (торец/цилиндр) с дугой. Сдвиг по
                // оси выровненного соседа через ArcAnchor.Sharp (общая формула с калькулятором
                // "Прочее"), для наружного И внутреннего точения. Касательный (или почти
                // касательный) вход/выход дуги — отдельная простая формула TangentialOffset.
                if (incomingIsArc != outgoingIsArc)
                {
                    var arc = outgoingIsArc ? (Arc)next! : (Arc)curr;
                    GDirection? arcTangent;
                    GDirection? straightDir;
                    if (outgoingIsArc)
                    {
                        arcTangent = TangentOutOf(nodes, i);
                        straightDir = i == 0 ? VirtualFaceDirection : TangentInto(nodes, i);
                    }
                    else
                    {
                        arcTangent = TangentInto(nodes, i);
                        straightDir = TangentOutOf(nodes, i);
                    }

                    if (arcTangent is not null && straightDir is not null &&
                        (IsFaceDirection(straightDir.Value) || IsCylinderDirection(straightDir.Value)))
                    {
                        var kind = IsFaceDirection(straightDir.Value) ? ArcAnchorKind.Face : ArcAnchorKind.Cylinder;
                        var angle = Math.Atan2(Math.Abs(arcTangent.Value.X), Math.Abs(arcTangent.Value.Z)).Degrees();
                        var tangential = kind == ArcAnchorKind.Face
                            ? angle >= ArcAnchor.SharpAngleGuardHigh
                            : angle <= ArcAnchor.SharpAngleGuardLow;
                        if (tangential)
                        {
                            var offset = TangentialOffset(kind, toolRadius, external);
                            result.Add(CompensateNode(curr, curr.X.Value + offset.X, curr.Z.Value + offset.Z));
                            continue;
                        }
                        var anchor = kind == ArcAnchorKind.Face ? curr.Z.Value : curr.X.Value;
                        var sharp = ArcAnchor.Sharp(kind, anchor, arc.CenterX, arc.CenterZ, arc.Radius, toolRadius, external);
                        if (sharp is not null)
                        {
                            result.Add(CompensateNode(curr, sharp.Value.X, sharp.Value.Z));
                            continue;
                        }
                    }
                }

                // Стык дуга-дуга: та же модель мнимой вершины, что для прямой-прямой, но по
                // касательным обеих дуг в точке стыка (TangentInto/TangentOutOf уже возвращают
                // касательную для дуги). Касательный стык (почти параллельные касательные) даёт
                // IntersectLines=null — узел остаётся на месте, радиусы скорректирует AdjustArcRadii.
                if (incomingIsArc && outgoingIsArc)
                {
                    var inDir = TangentInto(nodes, i);
                    var outDir = TangentOutOf(nodes, i);
                    if (inDir is null || outDir is null) { result.Add(curr); continue; }
                    var compensated = CornerVertex(new GPoint(curr.X.Value, curr.Z.Value), inDir.Value, outDir.Value, toolRadius, external);
                    if (compensated is not null)
                    {
                        result.Add(CompensateNode(curr, compensated.Value.X, compensated.Value.Z));
                        continue;
                    }
                }

                result.Add(curr);
            }

            return (result, finalizedArcs);
        }

        private static bool IsFaceDirection(GDirection d) => Math.Abs(d.Z) < Tolerance;

        private static bool IsCylinderDirection(GDirection d) => Math.Abs(d.X) < Tolerance;

        /// <summary>Выпуклая к инструменту дуга (радиус траектории вершины += радиус пластины) —
        /// когда её центр лежит со стороны материала, а не воздуха. Для наружного точения это
        /// дуги CCW, для внутреннего — CW (в координатах «X вверх, Z вправо»); выводится из
        /// <see cref="Arc.Direction"/> и стороны материала, совпадает с правилом радиуса в
        /// <c>Arc2Component.FullRadius</c> (выпуклая дуга, радиус растёт).</summary>
        private static bool IsConvexToTool(Arc arc, bool external)
            => external ? arc.Direction == Direction.CCW : arc.Direction == Direction.CW;

        /// <summary>Пересборка узла с новыми координатами без потери типа и параметров Blunt/дуги.</summary>
        private static Element CompensateNode(Element node, double x, double z)
            => node switch
            {
                Point p => new Point(x, z, p.Blunt),
                Line l => new Line(x, z, l.Angle, l.Blunt, l.BluntType),
                Arc a => new Arc(x, z, a.Radius, a.Direction, a.CenterX, a.CenterZ),
                _ => node,
            };

        /// <summary>Вершина мнимой вершины на стыке двух «прямых» направлений (реальные кромки или
        /// касательные дуг): каждая сдвигается вдоль своей нормали n=(dir.Z,-dir.X) на toolRadius,
        /// пересечение двух сдвинутых прямых — центр окружности, касательной обеим; вершина =
        /// center+(+R,-R) для внутреннего, center+(-R,-R) для наружного. Null при почти
        /// параллельных направлениях.</summary>
        private static GPoint? CornerVertex(GPoint point, GDirection inDir, GDirection outDir, double toolRadius, bool external)
        {
            var nIn = new GDirection(inDir.Z, -inDir.X);
            var nOut = new GDirection(outDir.Z, -outDir.X);
            var p1 = new GPoint(point.X + toolRadius * nIn.X, point.Z + toolRadius * nIn.Z);
            var p2 = new GPoint(point.X + toolRadius * nOut.X, point.Z + toolRadius * nOut.Z);
            var center = IntersectLines(p1, inDir, p2, outDir);
            if (center is null) return null;
            return external
                ? new GPoint(center.Value.X - toolRadius, center.Value.Z - toolRadius)
                : new GPoint(center.Value.X + toolRadius, center.Value.Z - toolRadius);
        }

        /// <summary>Простой (без тригонометрии) сдвиг для касательного входа/выхода дуги на
        /// прямом соседе: по оси соседа на радиус пластины — торец по X (к оси для наружного, от
        /// оси для внутреннего), цилиндр по Z (к -Z независимо от стороны материала, т.к.
        /// направление реза к -Z одинаково). Совпадает с <c>Arc2Component.StartDiamWithShift</c>.</summary>
        private static GPoint TangentialOffset(ArcAnchorKind kind, double toolRadius, bool external)
            => kind == ArcAnchorKind.Face
                ? new GPoint(external ? -toolRadius : toolRadius, 0)
                : new GPoint(0, -toolRadius);

        // ------------------------------------------------------------------
        // Радиусы дуг: ± радиус пластины (всегда, независимо от выравнивания соседей), кроме уже
        // финализированных маленьких дуг сопряжения из CompensateCorners (их радиус — уже точный
        // Blunt + toolRadius, повторно прибавлять toolRadius нельзя).
        // ------------------------------------------------------------------

        private static void AdjustArcRadii(List<Element> nodes, double toolRadius, bool external, HashSet<int> finalizedArcs)
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] is not Arc arc || finalizedArcs.Contains(i)) continue;
                var convex = IsConvexToTool(arc, external);
                var newRadius = convex ? arc.Radius + toolRadius : arc.Radius - toolRadius;
                if (newRadius <= 0) continue; // пластина не влезает в вогнутую дугу — оставляем номинальный радиус
                nodes[i] = new Arc(arc.X, arc.Z, newRadius, arc.Direction, arc.CenterX, arc.CenterZ);
            }
        }

        // ------------------------------------------------------------------
        // Геометрические примитивы и служебные функции
        // ------------------------------------------------------------------

        private readonly record struct GPoint(double X, double Z)
        {
            public static GPoint operator -(GPoint a, GPoint b) => new(a.X - b.X, a.Z - b.Z);
            public static GPoint operator +(GPoint a, GPoint b) => new(a.X + b.X, a.Z + b.Z);
        }

        private readonly record struct GDirection(double X, double Z);

        private static GDirection? TangentInto(List<Element> nodes, int index)
        {
            if (index <= 0) return null;
            var prev = nodes[index - 1];
            var curr = nodes[index];
            if (prev.X is null || prev.Z is null || curr.X is null || curr.Z is null) return null;
            // Сегмент, приходящий в узел index, описан данными самого curr (если curr — Arc, это
            // она рисует отрезок prev→curr, см. GCodeBuilder.Contour), а не prev.
            if (curr is Arc arc)
            {
                var radial = new GPoint(curr.X.Value, curr.Z.Value) - new GPoint(arc.CenterX, arc.CenterZ);
                return Normalize(new GDirection(-radial.Z, radial.X), arc.Direction);
            }
            return Normalize(new GDirection(curr.X.Value - prev.X.Value, curr.Z.Value - prev.Z.Value));
        }

        private static GDirection? TangentOutOf(List<Element> nodes, int index)
        {
            if (index >= nodes.Count - 1) return null;
            var curr = nodes[index];
            var next = nodes[index + 1];
            if (curr.X is null || curr.Z is null || next.X is null || next.Z is null) return null;
            if (next is Arc arc)
            {
                var radial = new GPoint(curr.X.Value, curr.Z.Value) - new GPoint(arc.CenterX, arc.CenterZ);
                return Normalize(new GDirection(-radial.Z, radial.X), arc.Direction);
            }
            return Normalize(new GDirection(next.X.Value - curr.X.Value, next.Z.Value - curr.Z.Value));
        }

        private static GDirection? Normalize(GDirection d, Direction? arcDirection = null)
        {
            var len = Math.Sqrt(d.X * d.X + d.Z * d.Z);
            if (len < Tolerance) return null;
            var n = new GDirection(d.X / len, d.Z / len);
            if (arcDirection == Direction.CCW) n = new GDirection(-n.X, -n.Z);
            return n;
        }

        /// <summary>Пересечение двух прямых, заданных точкой и направлением. Null, если прямые
        /// (почти) параллельны.</summary>
        private static GPoint? IntersectLines(GPoint p1, GDirection d1, GPoint p2, GDirection d2)
        {
            var denom = d1.X * d2.Z - d1.Z * d2.X;
            if (Math.Abs(denom) < Tolerance) return null;
            var t1 = ((p2.X - p1.X) * d2.Z - (p2.Z - p1.Z) * d2.X) / denom;
            return new GPoint(p1.X + t1 * d1.X, p1.Z + t1 * d1.Z);
        }

        private static bool IsAxisAligned(Element a, Element b)
        {
            if (a.X is null || a.Z is null || b.X is null || b.Z is null) return true;
            if (b is Arc) return false;
            var dx = Math.Abs(b.X.Value - a.X.Value);
            var dz = Math.Abs(b.Z.Value - a.Z.Value);
            return dx < Tolerance || dz < Tolerance;
        }
    }
}
