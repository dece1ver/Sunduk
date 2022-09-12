using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning.Base
{
    public class ThreadingTool : Tool
    {
        public double Pitch { get; set; }
        public double Angle { get; set; }

        public ThreadingTool(int position, double pitch, double angle, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Name = "REZBA";
            Pitch = pitch;
            Angle = angle;
            Hand = hand;
        }
    }
}
