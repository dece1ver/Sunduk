using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class ThreadCuttingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public ThreadingTool Tool { get; set; }
        public ThreadStandard ThreadStandard { get; set; }
        public CuttingType Type { get; set; }
        public double ThreadDiameter { get; set; }
        public double ThreadPitch { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public double ThreadNptPlane { get; set; }
        public string StandardTemplate { get; set; }
        public override string Operation => Templates.ThreadOperation.ThreadCutting(Machine, Tool, ThreadStandard, Type, ThreadDiameter, ThreadPitch, StartZ, EndZ, ThreadNptPlane);
        public override MachineType MachineType => MachineType.Turning;
        public override string Name
        {
            get
            {
                var name = "Точение резьбы";
                name += ThreadStandard switch
                {
                    ThreadStandard.Metric => $" M{ThreadDiameter.NC(option: Util.NcDecimalPointOption.Without)}x{Tool.Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.BSPP => $" {StandardTemplate}",
                    ThreadStandard.Trapezoidal => $" Tr{ThreadDiameter.NC(option: Util.NcDecimalPointOption.Without)}x{Tool.Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.NPT => $" {StandardTemplate}",
                    _ => string.Empty
                };
                return name;
            }
        }
        public override OperationTime MachineTime => this.OperationTime();

        public ThreadCuttingSequence(Machine machine, ThreadingTool tool, ThreadStandard threadStandard, CuttingType type, double threadDiameter, double threadPitch, double startZ, double endZ, double threadNptPlane, string standardTemplate = "")
        {
            Machine = machine;
            Tool = tool;
            ThreadStandard = threadStandard;
            Type = type;
            ThreadDiameter = threadDiameter;
            ThreadPitch = threadPitch;
            StartZ = startZ;
            EndZ = endZ;
            ThreadNptPlane = threadNptPlane;
            StandardTemplate = standardTemplate; 
        }
    }
}
