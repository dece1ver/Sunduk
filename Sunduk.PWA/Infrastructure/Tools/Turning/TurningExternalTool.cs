using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningExternalTool : TurningTool
    {
        public enum Types { Face, Bar }

        public Types Type { get; set; }
        public override string Name => Type == Types.Face ? "TORC" : "PROHOD";

        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public TurningExternalTool(int position, Types type, double angle, double radius, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Angle = angle;
            Radius = radius;
            Type = type;
            Hand = hand;
        }
    }
}
