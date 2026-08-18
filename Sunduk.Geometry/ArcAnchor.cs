using System;

namespace Sunduk.Geometry
{
    /// <summary>Вид прямого соседа дуги в стыке: торец (Z=const, направление вдоль X) или цилиндр
    /// (X=const, направление вдоль Z).</summary>
    public enum ArcAnchorKind
    {
        Face,
        Cylinder,
    }

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

        /// <summary>Острые (без скругления) стыки дуга-прямая под углом от 1° до 89° от оси Z —
        /// вне этого диапазона (почти касательный вход/выход) работает отдельная простая формула,
        /// а <see cref="Sharp"/> намеренно возвращает null.</summary>
        public const double SharpAngleGuardLow = 1;
        public const double SharpAngleGuardHigh = 89;

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

        /// <summary>
        /// Острый (без скругления) стык прямой (торец/цилиндр) с дугой — вершина мнимой вершины
        /// инструмента напрямую переходит в (увеличенную на радиус пластины) дугу, отдельная
        /// маленькая дуга не вставляется. Возвращает скомпенсированную точку на прямой (истинный
        /// радиус, не диаметр). Угол касательной дуги в точке стыка от оси Z + <see cref="GeometryMath.ChamferShifts"/>
        /// — конструкция, проверенная для торца (сверено с NippleComponent); для цилиндра — тот же
        /// расчёт, применённый к тому же тангенсу (никакого разворота осей местами не требуется,
        /// «зеркальность» получается сама за счёт вектора касательной). Сторона материала
        /// (<paramref name="external"/>) влияет только на знак сдвига по X (для торца): наружное —
        /// к оси, внутреннее — от оси; сдвиг по Z (для цилиндра) от стороны материала не зависит
        /// (направление реза к -Z одинаково). Null вне диапазона 1°..89° (почти касательный вход) —
        /// там отдельная простая формула, либо если прямая не достаёт дугу.
        /// </summary>
        public static (double X, double Z)? Sharp(ArcAnchorKind kind, double anchor, double arcCenterX, double arcCenterZ, double arcRadius, double toolRadius, bool external)
        {
            if (arcRadius <= 0) return null;
            double px, pz; // точка на прямой (торец/цилиндр) в истинных радиусных координатах
            if (kind == ArcAnchorKind.Face)
            {
                var catet = anchor - arcCenterZ;
                var relSq = arcRadius * arcRadius - catet * catet;
                if (relSq < 0) return null;
                px = arcCenterX + Math.Sqrt(relSq);
                pz = anchor;
            }
            else
            {
                var catet = anchor - arcCenterX;
                var relSq = arcRadius * arcRadius - catet * catet;
                if (relSq < 0) return null;
                px = anchor;
                pz = arcCenterZ + Math.Sqrt(relSq);
            }

            var radialX = px - arcCenterX;
            var radialZ = pz - arcCenterZ;
            var tangentX = -radialZ;
            var tangentZ = radialX;
            if (Math.Abs(tangentX) < Tolerance && Math.Abs(tangentZ) < Tolerance) return null;
            var angle = Math.Atan2(Math.Abs(tangentX), Math.Abs(tangentZ)).Degrees();
            if (angle <= SharpAngleGuardLow || angle >= SharpAngleGuardHigh) return null;

            var shift = GeometryMath.ChamferShifts(angle, toolRadius);
            var signX = external ? -1 : 1;
            return kind == ArcAnchorKind.Face
                ? (px + signX * shift.Z, pz)
                : (px, pz - shift.Z);
        }
    }
}
