namespace Sunduk.PWA.Infrastructure.Tools
{
    public class ThreadingExternalTool : Tool
    {
        public double Pitch { get; set; }
        public double Angle { get; set; }

        public ThreadingExternalTool(int position, double pitch, double angle)
        {
            Position = position;
            Name = "REZBA";
            Pitch = pitch;
            Angle = angle;
        }
    }
}
