using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingExternalTool : Tool
    {
        public enum Types { Grooving, Cutting, Blade }

        public Types Type { get; set; }
        public double Width { get; set; }
        public enum Point { Left, Right }
        public Point ZeroPoint { get; set; }
        public override string Name
        {
            get => Type switch
            {
                Types.Grooving => "KANAVA",
                Types.Cutting => "OTR",
                Types.Blade => "LEZVIE",
                _ => string.Empty,
            };
        }

        public GroovingExternalTool(int position, Types type, double width, Point zeroPoint, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Type = type;
            Width = width;
            ZeroPoint = zeroPoint;
            Hand = hand;
        }
    }
}
