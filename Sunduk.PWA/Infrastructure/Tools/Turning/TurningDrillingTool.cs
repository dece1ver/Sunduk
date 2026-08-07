using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningDrillingTool : DrillingTool 
    {
        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} D{Diameter})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} D{Diameter})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} D{Diameter})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} D{Diameter})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public TurningDrillingTool(int position, Types type, double diameter, double angle, ToolHand hand = ToolHand.Right)
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
