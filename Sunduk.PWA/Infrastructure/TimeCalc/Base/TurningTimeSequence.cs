using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.TimeCalc
{
    public class TurningTimeSequence
    {
        public double CutSpeed { get; set; }
        public double CutFeed { get; set; }
        public virtual double MachiningTime { get; set; }
        public virtual TurningTimeSequenceType Type { get; set; }

        public TurningTimeSequence(double cutSpeed, double cutFeed)
        {
            CutSpeed = cutSpeed;
            CutFeed = cutFeed;
        }
    }
}
