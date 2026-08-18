using System;

namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public abstract class TurningTool : Tool
    {
        public double Radius { get; set; }
        public double Angle { get; set; }

        /// <summary>Вектор мнимой вершины (см. <see cref="ToolNoseVector"/>) — определяет сторону
        /// материала при коррекции на радиус пластины (<see cref="Geometry.ToolTipCompensation"/>).
        /// Getter-only: выводится из конкретного типа инструмента, не сериализуется.</summary>
        public virtual ToolNoseVector NoseVector => ToolNoseVector.TurningExternal;

        public override MachineType MachineType => MachineType.Turning;

    }
}
