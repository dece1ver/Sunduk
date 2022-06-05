using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.TimeCalc
{
    public sealed class TurningTimeRoughSequence : TurningTimeSequence
    {
        public double StartDiameter { get; set; }
        public double EndDiameter { get; set; }
        public double StepOver { get; set; }
        public double Length { get; set; }
        public override double MachiningTime => (60 * Math.PI * Length * (StartDiameter + EndDiameter) / 2 * 1000 * CutFeed * CutSpeed) * ((StartDiameter - EndDiameter) / 2 * StepOver);
        public TurningTimeRoughSequence(double cutSpeed, double cutFeed, double length) : base(cutSpeed, cutFeed)
        {
            Type = TurningTimeSequenceType.RoughTurning;
            Length = length;
        }
    }
}
