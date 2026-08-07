using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningInternalTool : TurningTool
    {
        public double Diameter { get; set; }

        public override string Name => "RAST";

        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} D{Diameter} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} D{Diameter} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} D{Diameter} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} D{Diameter} {Angle} R{Radius})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public TurningInternalTool(int position, double diameter, double angle, double radius, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Diameter = diameter;
            Radius = radius;
            Angle = angle;
            Hand = hand;
        }
    }
}
