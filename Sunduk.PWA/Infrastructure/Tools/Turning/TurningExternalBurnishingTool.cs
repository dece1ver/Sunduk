using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningExternalBurnishingTool : TurningBurnishingTool
    {
        public TurningExternalBurnishingTool(int position, Types type, ToolHand hand = ToolHand.Right) : base(position, type, hand)
        {
        }
    }
}
