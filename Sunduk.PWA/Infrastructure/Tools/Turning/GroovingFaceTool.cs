using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingFaceTool : TurningGroovingTool
    {
        public override string Name => $"KANAVA TORC";

        public override MachineType MachineType => MachineType.Turning;

        public GroovingFaceTool(
            int position,
            double width,
            Point zeroPoint,
            ToolHand hand = ToolHand.Rigth,
            double cornerRadius = 0.2)
            : base(position, width, zeroPoint, hand, cornerRadius)
        {

        }
    }
}
    

