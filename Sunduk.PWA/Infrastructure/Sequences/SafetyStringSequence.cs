using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class SafetyStringSequence : Sequence
    {
        public Machine Machine { get; set; }
        public int? SpeedLimit { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override string Operation => Templates.Operation.SafetyString(Machine, SpeedLimit, CoordinateSystem);
        public override MachineType MachineType => MachineType.Any;
        public override OperationTime MachineTime => new(0, 0);

        public SafetyStringSequence(Machine machine, int speedLimit, CoordinateSystem coordinateSystem)
        {
            Machine = machine;
            SpeedLimit = speedLimit;
            CoordinateSystem = coordinateSystem;
        }
    }
}
