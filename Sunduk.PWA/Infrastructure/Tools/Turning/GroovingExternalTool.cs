using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingExternalTool : TurningGroovingTool
    {
        public enum Types { Grooving, Cutting, Blade }
        public Types Type { get; set; }
        public override string Name =>
            Type switch
            {
                Types.Grooving => "KANAVA",
                Types.Cutting => "OTR",
                Types.Blade => "LEZVIE",
                _ => string.Empty,
            };

        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} {Width}MM {(ZeroPoint == Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} {Width}MM {(ZeroPoint == Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} {Width}MM {(ZeroPoint == Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} {Width}MM {(ZeroPoint == Point.Left ? "KAK PROHOD" : "KAK OTR")})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public GroovingExternalTool(
            int position, 
            Types type, 
            double width,
            Point zeroPoint, 
            ToolHand hand = ToolHand.Right, 
            double cornerRadius = 0.2) 
            : base (position, width, zeroPoint, hand, cornerRadius)
        {
            Type = type;
        }
    }
}
