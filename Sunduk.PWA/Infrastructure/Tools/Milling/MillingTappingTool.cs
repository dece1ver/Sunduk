using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingTappingTool : TappingTool
    {
        public override MachineType MachineType => MachineType.Milling;

        public MillingTappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Right)
            : base(position, type, diameter, pitch, hand)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            Hand = hand;
        }
    }
}
