using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class ThreadMillingSequence : Sequence
    {
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public double SafePlane { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public Machine Machine { get; set; }
        public MillingThreadCuttingTool Tool { get; set; }
        public double ThreadDiameter { get; set; }
        public double CutSpeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }

        public override string Name
        {
            get
            {
                var name = "Резьбофрезерование";
                name += Tool.ThreadStandard switch
                {
                    ThreadStandard.Metric => $" M{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Tool.Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.BSPP => $" {Tool.StandardTemplate}",
                    ThreadStandard.Trapezoidal => $" Tr{Tool.Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Tool.Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.NPT => $" {Tool.StandardTemplate}",
                    _ => string.Empty
                };
                return name;
            }
        }

        public override OperationTime MachineTime => this.OperationTime();
        public ThreadMillingSequence(Machine machine, MillingThreadCuttingTool tool, double threadDiameter, double cutSpeed, double startZ, double endZ)
        {
            Machine = machine;
            Tool = tool;
            ThreadDiameter = threadDiameter;
            CutSpeed = cutSpeed;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
