using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingInternalTool : TurningGroovingTool
    {
        public double Diameter { get; set; }
        public override string Name => $"KANAVA";

        public override MachineType MachineType { get => MachineType.Turning; }

        public GroovingInternalTool(
            int position,
            double diameter,
            double width,
            Point zeroPoint,
            ToolHand hand = ToolHand.Rigth,
            double cornerRadius = 0.2)
            : base(position, width, zeroPoint, hand, cornerRadius)
        {
            Diameter = diameter;
        }
    }
}
