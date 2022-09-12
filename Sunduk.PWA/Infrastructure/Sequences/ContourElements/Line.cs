using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.ContourElements
{
    public sealed class Line : Element
    {
        public Line(double? x, double? z, double angle = 0, double blunt = 0, Blunt bluntType = Infrastructure.Blunt.Radius)
        {
            X = x;
            Z = z;
            Angle = angle;
            Blunt = blunt;
            BluntType = bluntType;
        }

        public override double? X { get; set; }
        public override double? Z { get; set; }
        public double Angle { get; set; }
        public double Blunt { get; set; }
        public Blunt BluntType { get; set; }
    }
}
