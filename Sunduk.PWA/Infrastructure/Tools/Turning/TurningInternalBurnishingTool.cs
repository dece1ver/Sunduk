using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class TurningInternalBurnishingTool : TurningBurnishingTool
    {
        public double Diameter { get; set; }
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} D{Diameter.NC(option: Util.NcDecimalPointOption.Without)})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };
        public TurningInternalBurnishingTool(int position, Types type, double diameter, ToolHand hand = ToolHand.Right) : base(position, type, hand)
        {
            Diameter = diameter;
        }
    }
}
