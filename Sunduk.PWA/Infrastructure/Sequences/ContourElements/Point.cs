using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public class Point : Element
    {
        public Point(double? x, double? z)
        {
            X = x;
            Z = z;
        }

        public double? X { get; set; }
        public double? Z { get; set; }
    }
}
