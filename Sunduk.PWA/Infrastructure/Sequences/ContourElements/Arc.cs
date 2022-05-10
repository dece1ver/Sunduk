using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public class Arc : Element
    {
        public Arc(double x, double z, double radius, double centerX = 0, double centerZ = 0)
        {
            X = x;
            Z = z;
            Radius = radius;
            CenterX = centerX;
            CenterZ = centerZ;
        }

        public double X { get; set; }
        public double Z { get; set; }
        public double Radius { get; set; }
        public double CenterX { get; set; }
        public double CenterZ { get; set; }
    }
}
