using Sunduk.Geometry.ContourElements.Base;

namespace Sunduk.Geometry.ContourElements
{
    public sealed class Point : Element
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
