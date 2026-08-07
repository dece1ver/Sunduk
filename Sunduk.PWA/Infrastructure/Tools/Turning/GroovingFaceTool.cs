using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingFaceTool : TurningGroovingTool
    {
        public override string Name => $"KANAVA TORC";

        public override MachineType MachineType => MachineType.Turning;
        public override string Description(Util.ToolDescriptionOption option) => option switch
        {
            Util.ToolDescriptionOption.General => $"T{Position.ToolNumber()} ({Name} {Width}MM {(ZeroPoint == Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
            Util.ToolDescriptionOption.L230 => $"T{Position.ToolNumber()} ({Name} {Width}MM {(ZeroPoint == Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayLeft => $"T{Position.ToolNumber()} G54 M58 ({Name} {Width}MM {(ZeroPoint == Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
            Util.ToolDescriptionOption.GoodwayRight => $"T{Position.ToolNumber()} G55 M58 ({Name} {Width}MM {(ZeroPoint == Point.Bottom ? "KAK PROHOD" : "KAK RAST")})".Replace(',', '.'),
            Util.ToolDescriptionOption.ToolTable => Description(Util.ToolDescriptionOption.General).Split('(')[1].TrimEnd(')'),
            _ => string.Empty,
        };

        public GroovingFaceTool(
            int position,
            double width,
            Point zeroPoint,
            ToolHand hand = ToolHand.Right,
            double cornerRadius = 0.2)
            : base(position, width, zeroPoint, hand, cornerRadius)
        {

        }
    }
}
    

