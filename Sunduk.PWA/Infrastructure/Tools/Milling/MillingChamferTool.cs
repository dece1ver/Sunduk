using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingChamferTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Name => "FASKA";
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position:D2} ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Angle.NC(option: Util.NcDecimalPointOption.Without)})",
            Util.ToolDescriptionOption.MillingToolChange => $"T{Position} M6 ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Angle.NC(option: Util.NcDecimalPointOption.Without)})",
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public double TipCompensation { get; set; }

        public MillingChamferTool(int position, double diameter, double angle, double tipCompensation = 0, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Diameter = diameter;
            Angle = angle;
            TipCompensation = tipCompensation;
            Hand = hand;
        }
    }
}
