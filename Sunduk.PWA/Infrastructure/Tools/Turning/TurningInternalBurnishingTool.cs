using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningInternalBurnishingTool : TurningBurnishingTool
    {
        public double Diameter { get; set; }
        public TurningInternalBurnishingTool(int position, Types type, double diameter, ToolHand hand = ToolHand.Right) : base(position, type, hand)
        {
            Diameter = diameter;
        }
    }
}
