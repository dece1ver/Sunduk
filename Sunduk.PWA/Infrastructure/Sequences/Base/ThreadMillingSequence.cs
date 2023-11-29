using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class ThreadMillingSequence : Sequence
    {
        
        public CoordinateSystem CoordinateSystem { get; set; }
        public override MachineType MachineType => MachineType.Milling;
        public Machine Machine { get; set; }
        public MillingThreadCuttingTool Tool { get; set; }
        public double ThreadDiameter { get; set; }
        public double CutSpeed { get; set; }
        public double CutFeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }
        public int RoughPasses { get; set; }
        public double RoughStepOver { get; set; }
        public double ProfStockAllow { get; set; }
        public double ExitPlane { get; set; }
        public bool FullCut { get; set; }
        public List<Hole> Holes { get; set; }
        public bool Polar { get; set; }
        public double SafePlane { get; set; }

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
        public ThreadMillingSequence(
            Machine machine,
            CoordinateSystem coordinateSystem,
            MillingThreadCuttingTool tool,
            double threadDiameter,
            double cutSpeed,
            double cutFeed,
            double startZ,
            double endZ,
            int roughPasses,
            double roughStepOver,
            double profStockAllow,
            double exitPlane,
            bool fullCut,
            List<Hole> holes,
            bool polar,
            double safePlane)
        {
            Machine = machine;
            CoordinateSystem = coordinateSystem;
            Tool = tool;
            ThreadDiameter = threadDiameter;
            CutSpeed = cutSpeed;
            CutFeed = cutFeed;
            StartZ = startZ;
            EndZ = endZ;
            RoughPasses = roughPasses;
            RoughStepOver = roughStepOver;
            ProfStockAllow = profStockAllow;
            ExitPlane = exitPlane;
            FullCut = fullCut;
            Holes = holes;
            Polar = polar;
            SafePlane = safePlane;
        }

    }
}
