using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public class MillingDrillingTool : DrillingTool 
    {
        public MillingDrillingTool(int position, Types type, double diameter, double angle, ToolHand hand = ToolHand.Rigth)
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
