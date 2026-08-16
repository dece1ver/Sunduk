using System;

namespace Sunduk.Geometry
{
    /// <summary>
    /// Чистая тригонометрическая/угловая математика, вынесенная из <c>Calc</c> в эту библиотеку,
    /// чтобы геометрия (<see cref="ToolTipCompensation"/>, <see cref="ArcAnchor"/>) не зависела от
    /// UI/доменных сборок. Формулы и сигнатуры не менялись при переносе.
    /// </summary>
    public static class GeometryMath
    {
        /// <summary>Переводит радианы в угол.</summary>
        public static double Degrees(this double radians) => radians * 180 / Math.PI;

        /// <summary>Переводит угол в радианы.</summary>
        public static double Radians(this double degrees) => degrees * Math.PI / 180;

        /// <summary>Переводит угол в радианы.</summary>
        public static double Radians(this int degrees) => degrees * Math.PI / 180;

        /// <summary>
        /// Смещение от виртуальных точек пересечения до концов радиусов.
        /// </summary>
        /// <param name="angle">Угол фаски от горизонтальной оси</param>
        /// <param name="radius">Радиус на углах фаски</param>
        public static (double X, double Z) ChamferRadiusLengths(double angle, double radius, int round = 3)
        {
            return (
                Math.Round(Math.Tan(((90 - angle) / 2).Radians()) * radius, round, MidpointRounding.ToPositiveInfinity),
                Math.Round(Math.Tan((angle / 2).Radians()) * radius, round, MidpointRounding.ToPositiveInfinity));
        }

        /// <summary>Смещение координат фаски с учетом радиуса пластины.</summary>
        public static (double X, double Z) ChamferShifts(double angle, double radius)
        {
            return
                (Math.Tan(angle.Radians()) * (radius - radius / Math.Tan((90 - angle / 2).Radians())),
                radius - radius / Math.Tan((90 - angle / 2).Radians()));
        }
    }
}
