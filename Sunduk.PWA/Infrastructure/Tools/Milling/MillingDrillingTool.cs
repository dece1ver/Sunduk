using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingDrillingTool : DrillingTool 
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position:D2} ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})",
            Util.ToolDescriptionOption.MillingToolChange => $"T{Position} M6 ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})",
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

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
