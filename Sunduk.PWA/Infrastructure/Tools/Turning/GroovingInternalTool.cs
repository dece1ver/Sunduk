using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class GroovingInternalTool : Tool
    {
        public enum Point { Left, Right }

        public double Diameter { get; set; }
        public double Width { get; set; }
        public Point ZeroPoint { get; set; }
        public override string Name => "KANAVA";

        public GroovingInternalTool(int position, double diameter, double width, Point zeroPoint, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Diameter = diameter;
            Width = width;
            ZeroPoint = zeroPoint;
            Hand = hand;
        }
    }
}
