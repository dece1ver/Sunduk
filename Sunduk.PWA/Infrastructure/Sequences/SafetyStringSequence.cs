using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class SafetyStringSequence : Sequence
    {
        public Machines Machine { get; set; }
        public int SpeedLimit { get; set; }
        public override string Operation => Templates.Operation.SafetyString(Machine, SpeedLimit);

        public SafetyStringSequence(Machines machine, int speedLimit)
        {
            Machine = machine;
            SpeedLimit = speedLimit;
        }
    }
}
