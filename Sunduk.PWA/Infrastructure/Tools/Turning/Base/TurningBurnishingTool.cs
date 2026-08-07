using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning.Base
{
    public class TurningBurnishingTool : Tool
    {
        public override MachineType MachineType => MachineType.Turning;
        public enum Types { Diamond, Roller }
        public Types Type { get; set; }
        public override string Name => "NAKATKA " + (Type is Types.Diamond ? "ALMAZ" : "ROLIK");
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public TurningBurnishingTool(int position, Types type, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Hand = hand;
        }
    }
}
