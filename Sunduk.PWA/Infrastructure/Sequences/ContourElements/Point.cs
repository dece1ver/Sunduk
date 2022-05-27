using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public class Point : Element
    {
        public Point(double? x, double? z, double blunt = 0)
        {
            X = x;
            Z = z;
            Blunt = blunt;
        }

        public override double? X { get; set; }
        public override double? Z { get; set; }
        public double Blunt { get; set; }
    }
}
