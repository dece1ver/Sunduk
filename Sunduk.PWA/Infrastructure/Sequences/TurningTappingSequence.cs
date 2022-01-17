using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class TurningTappingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public TurningTappingTool Tool { get; set; }
        public double CutSpeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public override string Operation => Templates.Operation.Tapping(Machine, Tool, CutSpeed, StartZ, EndZ);
        public override MachineType MachineType { get => MachineType.General; }

        public TurningTappingSequence(Machine machine, TurningTappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            Machine = machine;
            Tool = tool;
            CutSpeed = cutSpeed;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
