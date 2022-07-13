using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingTappingTool : TappingTool
    {
        public override MachineType MachineType => MachineType.Milling;

        public MillingTappingTool(int position, Types type, double diameter, double pitch, ThreadStandard threadStandard, string standardTemplate = "", ToolHand hand = ToolHand.Right)
            : base(position, type, diameter, pitch, threadStandard, standardTemplate, hand)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            ThreadStandard = threadStandard;
            StandardTemplate = standardTemplate;
            Hand = hand;
        }
    }
}
