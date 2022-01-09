using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class ThreadingExternalTool : Tool
    {
        public double Pitch { get; set; }
        public double Angle { get; set; }

        public ThreadingExternalTool(int position, double pitch, double angle, ToolHand hand = ToolHand.Rigth)
        {
            Position = position;
            Name = "REZBA";
            Pitch = pitch;
            Angle = angle;
            Hand = hand;
        }
    }
}
