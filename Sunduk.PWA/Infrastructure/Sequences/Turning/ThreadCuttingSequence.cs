using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
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
        public override string Operation => Templates.ThreadOperation.ThreadCutting(Machine, Tool, ThreadStandart, Type, ThreadDiameter, ThreadPitch, StartZ, EndZ, ThreadNPTPlane);
        public override MachineType MachineType => MachineType.Turning;
        public override string Name { get => $"Точение резьбы"; }

        public ThreadCuttingSequence(Machine machine, ThreadingTool tool, ThreadStandart threadStandart, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNPTPlane)
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
