using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public class Line : Element
    {
        public Line(double x, double z, double angle = 0, double startBlunt = 0, Blunt startBluntType = Blunt.Radius, double endBlunt = 0, Blunt endBluntType = Blunt.Radius)
        {
            X = x;
            Z = z;
            Angle = angle;
            StartBlunt = startBlunt;
            StartBluntType = startBluntType;
            EndBlunt = endBlunt;
            EndBluntType = endBluntType;
        }

        public double X { get; set; }
        public double Z { get; set; }
        public double Angle { get; set; }
        public double StartBlunt { get; set; }
        public Blunt StartBluntType { get; set; }
        public double EndBlunt { get; set; }
        public Blunt EndBluntType { get; set; }
    }
}
