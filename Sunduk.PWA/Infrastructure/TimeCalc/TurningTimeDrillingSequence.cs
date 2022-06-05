namespace Sunduk.PWA.Infrastructure.TimeCalc
{
    public sealed class TurningTimeDrillingSequence : TurningTimeSequence
    {
        public double Diameter { get; set; }
        public double Length { get; set; }
        public override double MachiningTime => 1;
        public TurningTimeDrillingSequence(double cutSpeed, double cutFeed, double length) : base(cutSpeed, cutFeed)
        {
            Type = TurningTimeSequenceType.RoughTurning;
            Length = length;
        }
    }
}
