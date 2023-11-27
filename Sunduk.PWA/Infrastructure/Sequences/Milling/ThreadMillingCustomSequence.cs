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
            CutSpeed, 
            StartZ, 
            EndZ, 
            Holes, 
            Polar, 
            SafePlane);
        public ThreadMillingCustomSequence(Machine machine, CoordinateSystem coordinateSystem, MillingThreadCuttingTool tool, double threadDiameter, double cutSpeed, double startZ, double endZ, List<Hole> holes, bool polar, double safePlane)
            : base(machine, tool, threadDiameter, cutSpeed, startZ, endZ)
        { }
    }
}
