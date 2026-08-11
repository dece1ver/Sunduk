using System;

namespace Sunduk.PWA.Infrastructure.Geometry
{
    /// <summary>
    /// Пересечение дуги (заданной центром и радиусом, в истинных радиусных координатах — не
    /// диаметр) с торцом (Z=const) или цилиндром (X=const), с опциональным скруглением на стыке
    /// (та же треугольная Пифагор-конструкция, что в `NippleComponent.GetBluntedShape`/
    /// `Arc2Component.GetBluntedShape` — при <paramref name="blunt"/>=0 сводится к обычному
    /// пересечению прямой с окружностью). Вынесено из <see cref="ToolTipCompensation"/> (где было
    /// приватным как <c>FaceArcBlunt</c>/<c>CylinderArcBlunt</c>) в общее место, чтобы той же
    /// формулой мог пользоваться и калькулятор произвольной дуги в "Прочее" — вместо второй копии
    /// той же математики. Поведение и сигнатуры результата не менялись при переносе.
    /// </summary>
    public static class ArcAnchor
    {
        private const double Tolerance = 1e-6;

        /// <summary>
        /// Торец на <paramref name="faceZ"/> встречает дугу (центр/радиус в радиусных
        /// координатах). Возвращает координаты (в истинном радиусе) точки на торце и точки
        /// передачи в дугу (её радиус уже увеличен на <paramref name="toolRadius"/>). Null, если
        /// геометрия не сходится (скругление больше, чем позволяет дуга/торец, либо дуга торец не
        /// достаёт).
        /// </summary>
        public static (double StartX, double EndX, double EndZ)? Face(double faceZ, double arcCenterX, double arcCenterZ, double arcRadius, double blunt, double toolRadius)
        {
            var cz0 = faceZ - arcCenterZ;
            var catet = cz0 - blunt;
            if (catet <= Tolerance) return null;
            var startXSq = Math.Pow(arcRadius - blunt, 2) - Math.Pow(catet, 2);
            if (startXSq < 0) return null;
            var startXRel = Math.Sqrt(startXSq);
            var angle = Math.Atan(startXRel / catet);
            var endXRel = blunt * Math.Sin(angle) + startXRel;
            var endZDepth = blunt - Math.Sqrt(Math.Max(0, Math.Pow(blunt, 2) - Math.Pow(blunt * Math.Sin(angle), 2)));
            if (toolRadius > 0)
            {
                startXRel -= toolRadius;
                var bluntR = blunt + toolRadius;
                var catet2 = bluntR * Math.Sin(angle);
                endXRel = catet2 + startXRel;
                endZDepth = bluntR - Math.Sqrt(Math.Max(0, Math.Pow(bluntR, 2) - Math.Pow(catet2, 2)));
            }
            return (arcCenterX + startXRel, arcCenterX + endXRel, faceZ - endZDepth);
        }

        /// <summary>
        /// Цилиндр на <paramref name="cylX"/> (истинный радиус) встречает дугу — зеркальное (X↔Z)
        /// обобщение <see cref="Face"/>. Возвращает координаты (в истинном радиусе) точки касания
        /// с цилиндром и точки передачи в дугу (её радиус уже увеличен на
        /// <paramref name="toolRadius"/>). Null, если геометрия не сходится.
        /// </summary>
        public static (double TangentZ, double HandoffX, double HandoffZ)? Cylinder(double cylX, double arcCenterX, double arcCenterZ, double arcRadius, double blunt, double toolRadius)
        {
            var cx0 = cylX - arcCenterX;
            var catet = cx0 - blunt;
            if (catet <= Tolerance) return null;
            var startZSq = Math.Pow(arcRadius - blunt, 2) - Math.Pow(catet, 2);
            if (startZSq < 0) return null;
            var startZRel = Math.Sqrt(startZSq);
            var angle = Math.Atan(startZRel / catet);
            var endZRel = blunt * Math.Sin(angle) + startZRel;
            var endXDepth = blunt - Math.Sqrt(Math.Max(0, Math.Pow(blunt, 2) - Math.Pow(blunt * Math.Sin(angle), 2)));
            if (toolRadius > 0)
            {
                startZRel -= toolRadius;
                var bluntR = blunt + toolRadius;
                var catet2 = bluntR * Math.Sin(angle);
                endZRel = catet2 + startZRel;
                endXDepth = bluntR - Math.Sqrt(Math.Max(0, Math.Pow(bluntR, 2) - Math.Pow(catet2, 2)));
            }
            return (arcCenterZ + startZRel, cylX - endXDepth, arcCenterZ + endZRel);
        }
    }
}
