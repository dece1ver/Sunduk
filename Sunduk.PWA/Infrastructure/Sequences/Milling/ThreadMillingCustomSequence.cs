using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class ThreadMillingCustomSequence: ThreadMillingSequence
    {
        
        public override string Operation => Templates.ThreadOperation.CustomThreadMilling(
            Machine, 
            CoordinateSystem, 
            Tool, 
            ThreadDiameter, 
            CutSpeed, 
            CutFeed, 
            StartZ, 
            EndZ, 
            RoughPasses,
            RoughStepOver,
            ProfStockAllow,
            ExitPlane,
            FullCut,
            Holes, 
            Polar, 
            SafePlane);
        public ThreadMillingCustomSequence(
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
            : base(machine, coordinateSystem, tool, threadDiameter, cutSpeed, cutFeed, startZ, endZ, roughPasses, roughStepOver, profStockAllow, exitPlane, fullCut, holes, polar, safePlane)
        { }
    }
}
