using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class ThreadCuttingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Tool Tool { get; set; }
        public ThreadStandart ThreadStandart { get; set; }
        public CuttingType Type { get; set; }
        public double ThreadDiameter { get; set; }
        public double ThreadPitch { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public double ThreadNPTPlane { get; set; }
        public override string Operation => Templates.Operation.ThreadCutting(Machine, Tool, ThreadStandart, Type, ThreadDiameter, ThreadPitch, StartZ, EndZ, ThreadNPTPlane);

        public ThreadCuttingSequence(Machine machine, Tool tool, ThreadStandart threadStandart, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNPTPlane)
        {
            Machine = machine;
            Tool = tool;
            ThreadStandart = threadStandart;
            Type = type;
            ThreadDiameter = threadDiameter;
            ThreadPitch = threadPitch;
            StartZ = startZ;
            EndZ = endZ;
            ThreadNPTPlane = threadNPTPlane;
        }
    }
}
