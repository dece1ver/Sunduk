using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class ThreadingInternalTool : ThreadingTool
    {
        public double Diameter { get; set; }

        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} D{Diameter} {Pitch} {Angle})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} D{Diameter} {Pitch} {Angle})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} D{Diameter} {Pitch} {Angle})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} D{Diameter} {Pitch} {Angle})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public ThreadingInternalTool(int position, double diameter, double pitch, double angle, ToolHand hand = ToolHand.Right)
            : base(position, pitch, angle, hand)
        {
            Diameter = diameter;
        }
    }
}
