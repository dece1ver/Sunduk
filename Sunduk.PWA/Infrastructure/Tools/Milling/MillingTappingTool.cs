using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public class MillingTappingTool : TappingTool
    {
        public override MachineType MachineType { get => MachineType.Milling; }

        public MillingTappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Rigth)
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
