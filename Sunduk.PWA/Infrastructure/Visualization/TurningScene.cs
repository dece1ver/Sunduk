using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sunduk.Geometry;

namespace Sunduk.PWA.Infrastructure.Visualization
{
    /// <summary>
    /// Сцена XZ для превью токарного перехода: заготовка (полусечение выше оси) + траектория,
    /// спроецированные в виртуальный холст <see cref="Width"/> x <see cref="Height"/>. Холст общий
    /// для всего — SVG масштабируется целиком (viewBox), поэтому толщина линий и размер шрифта
    /// задаются прямо в единицах холста и не зависят от размеров детали; сам масштаб
    /// (<see cref="Scale"/>, единиц холста на мм) подбирается так, чтобы вписать габариты сцены.
    /// Горизонталь — Z (вправо Z+), вертикаль — радиус (вверх X+); X программы — диаметр, поэтому
    /// по вертикали откладывается X/2 в том же масштабе, что и Z (дуги остаются окружностями).
    /// </summary>
    public sealed class TurningScene
    {
        public const double Width = 640;
        private const double MinHeight = 240;
        private const double MaxHeight = 420;
        private const double PadX = 34;
        private const double PadTop = 60;   // сверху — место под значок системы координат
        private const double PadBottom = 30;

        public double Height { get; private set; } = MinHeight;
        public double Scale { get; private set; } = 1;

        private double _minZ, _maxR, _offsetX, _offsetY;

        public TurningToolpath? Toolpath { get; }
        public StockProfile? Stock { get; }

        /// <summary>Видимая по Z часть заготовки — [-длина, +припуск по торцу]; (0,0), если заготовка не задана.</summary>
        public (double From, double To) StockZRange { get; }
        public bool HasStock => Stock is not null && StockZRange.To > StockZRange.From;

        public bool Valid { get; }

        public TurningScene(TurningToolpath? toolpath, StockProfile? stock, double stockLength, double facingAllowance)
        {
            Toolpath = toolpath;
            Stock = stock;
            StockZRange = stock is not null && stockLength > 0 ? (-stockLength, Math.Max(facingAllowance, 0)) : (0, 0);

            var points = new List<Pt>();
            if (toolpath is not null) points.AddRange(toolpath.AllPoints());
            if (points.Count == 0 && !HasStock) return;

            // Кадр строится по траектории, а не по заготовке: длинная/толстая заготовка иначе делает
            // деталь мелкой и плоской. Заготовка просто обрезается по кадру.
            double minZ, maxZ, minR, maxR;
            if (points.Count > 0)
            {
                minZ = points.Min(p => p.Z); maxZ = points.Max(p => p.Z);
                minR = points.Min(p => p.R); maxR = points.Max(p => p.R);
            }
            else
            {
                minZ = StockZRange.From; maxZ = StockZRange.To; minR = 0;
                maxR = stock!.ExternalSegments.Where(s => s.Diameter > 0).Select(s => s.Diameter / 2).DefaultIfEmpty(1).Max();
            }
            // ось вращения включаем в кадр, только если она не слишком далеко от контура
            var contourSpan = Math.Max(Math.Max(maxZ - minZ, maxR - minR), 1);
            if (minR > 0 && minR <= contourSpan * 0.6) minR = 0;
            // воздух вокруг и минимальные размеры, чтобы вырожденный контур (одна линия) не растягивался
            var marginZ = Math.Max((maxZ - minZ) * 0.06, 0.5);
            var marginR = Math.Max((maxR - minR) * 0.06, 0.5);
            minZ -= marginZ; maxZ += marginZ; maxR += marginR;
            if (minR > 0) minR -= marginR;
            var spanZ = Math.Max(maxZ - minZ, 10);
            var spanR = Math.Max(maxR - minR, 6);
            // расширенную до минимума область центрируем
            minZ -= (spanZ - (maxZ - minZ)) / 2;
            maxR += (spanR - (maxR - minR)) / 2;

            var scale = (Width - 2 * PadX) / spanZ;
            var height = Math.Clamp(spanR * scale + PadTop + PadBottom, MinHeight, MaxHeight);
            scale = Math.Min(scale, (height - PadTop - PadBottom) / spanR);

            Scale = scale;
            Height = height;
            _minZ = minZ;
            _maxR = maxR;
            // центрируем по обеим осям, если запас остался
            _offsetX = (Width - spanZ * scale) / 2;
            _offsetY = PadTop + (height - PadTop - PadBottom - spanR * scale) / 2;
            Valid = true;
        }

        public double X(double z) => _offsetX + (z - _minZ) * Scale;
        public double Y(double r) => _offsetY + (_maxR - r) * Scale;

        public static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

        /// <summary>Ось (R=0) попадает в видимую область.</summary>
        public bool AxisVisible => Valid && Y(0) <= Height - 2 && Y(0) >= 2;

        /// <summary>SVG-путь по перемещениям: непрерывные участки идут одним подпутём, на разрыве — M.</summary>
        public string PathD(IEnumerable<PathMove> moves)
        {
            var sb = new StringBuilder();
            Pt? last = null;
            foreach (var m in moves)
            {
                if (last is null || Math.Abs(last.Value.Z - m.From.Z) > 1e-9 || Math.Abs(last.Value.R - m.From.R) > 1e-9)
                    sb.Append("M ").Append(F(X(m.From.Z))).Append(',').Append(F(Y(m.From.R))).Append(' ');
                if (m.IsArc)
                {
                    var r = F(m.Radius * Scale);
                    // экранные Y вниз, поэтому визуальное "по часовой" (G2) остаётся sweep=1
                    sb.Append("A ").Append(r).Append(',').Append(r).Append(" 0 0 ").Append(m.Direction == Direction.CW ? 1 : 0).Append(' ');
                }
                else sb.Append("L ");
                sb.Append(F(X(m.To.Z))).Append(',').Append(F(Y(m.To.R))).Append(' ');
                last = m.To;
            }
            return sb.ToString().TrimEnd();
        }

        /// <summary>Сплошные участки сегментов (диаметр &gt; 0), обрезанные по видимой части заготовки и
        /// склеенные по стыкующимся концам в "пробежки".</summary>
        private List<List<(double From, double To, double Diameter)>> Runs(IReadOnlyList<(double FromZ, double ToZ, double Diameter)> segments)
        {
            var clipped = segments
                .Select(s => (From: Math.Max(s.FromZ, StockZRange.From), To: Math.Min(s.ToZ, StockZRange.To), s.Diameter))
                .Where(s => s.To - s.From > 1e-9 && s.Diameter > 0)
                .OrderBy(s => s.From)
                .ToList();
            var runs = new List<List<(double, double, double)>>();
            foreach (var s in clipped)
            {
                if (runs.Count > 0 && Math.Abs(runs[^1][^1].Item2 - s.From) < 1e-6) runs[^1].Add(s);
                else runs.Add(new List<(double, double, double)> { s });
            }
            return runs;
        }

        private string HalfPolygon(List<(double From, double To, double Diameter)> run)
        {
            var sb = new StringBuilder();
            sb.Append("M ").Append(F(X(run[0].From))).Append(',').Append(F(Y(0))).Append(' ');
            foreach (var s in run)
            {
                sb.Append("L ").Append(F(X(s.From))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
                sb.Append("L ").Append(F(X(s.To))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
            }
            sb.Append("L ").Append(F(X(run[^1].To))).Append(',').Append(F(Y(0))).Append(" Z ");
            return sb.ToString();
        }

        /// <summary>Заливка заготовки: полусечение наружного профиля минус отверстия (evenodd).</summary>
        public string StockFillPath
        {
            get
            {
                if (!HasStock || !Valid) return string.Empty;
                var sb = new StringBuilder();
                foreach (var run in Runs(Stock!.ExternalSegments)) sb.Append(HalfPolygon(run));
                foreach (var run in Runs(Stock.InternalSegments)) sb.Append(HalfPolygon(run));
                return sb.ToString().TrimEnd();
            }
        }

        /// <summary>Обводка внешней границы заготовки (без стороны по оси).</summary>
        public string StockOutlinePath
        {
            get
            {
                if (!HasStock || !Valid) return string.Empty;
                var sb = new StringBuilder();
                foreach (var run in Runs(Stock!.ExternalSegments))
                {
                    sb.Append("M ").Append(F(X(run[0].From))).Append(',').Append(F(Y(0))).Append(' ');
                    foreach (var s in run)
                    {
                        sb.Append("L ").Append(F(X(s.From))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
                        sb.Append("L ").Append(F(X(s.To))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
                    }
                    sb.Append("L ").Append(F(X(run[^1].To))).Append(',').Append(F(Y(0))).Append(' ');
                }
                return sb.ToString().TrimEnd();
            }
        }

        /// <summary>Обводка отверстий (пунктир).</summary>
        public string HoleOutlinePath
        {
            get
            {
                if (!HasStock || !Valid) return string.Empty;
                var sb = new StringBuilder();
                foreach (var run in Runs(Stock!.InternalSegments))
                {
                    sb.Append("M ").Append(F(X(run[0].From))).Append(',').Append(F(Y(0))).Append(' ');
                    foreach (var s in run)
                    {
                        sb.Append("L ").Append(F(X(s.From))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
                        sb.Append("L ").Append(F(X(s.To))).Append(',').Append(F(Y(s.Diameter / 2))).Append(' ');
                    }
                    sb.Append("L ").Append(F(X(run[^1].To))).Append(',').Append(F(Y(0))).Append(' ');
                }
                return sb.ToString().TrimEnd();
            }
        }
    }
}
