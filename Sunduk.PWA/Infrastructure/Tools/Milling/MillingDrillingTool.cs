using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingDrillingTool : DrillingTool 
    {
        public override MachineType MachineType => MachineType.Milling;

        public MillingDrillingTool(int position, Types type, double diameter, double angle, ToolHand hand = ToolHand.Right)
            : base(position, type, diameter, angle, hand)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Angle = angle;
            Hand = hand;
        }
    }
}
