using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public class Arc : Element
    {
        public Arc(double? x, double? z, double radius, Direction direction, double centerX = 0, double centerZ = 0)
        {
            X = x;
            Z = z;
            Radius = radius;
            CenterX = centerX;
            CenterZ = centerZ;
            Direction = direction;
        }

        public override double? X { get; set; }
        public override double? Z { get; set; }
        public double Radius { get; set; }
        public double CenterX { get; set; }
        public double CenterZ { get; set; }
        public Direction Direction { get; set; }
    }
}
