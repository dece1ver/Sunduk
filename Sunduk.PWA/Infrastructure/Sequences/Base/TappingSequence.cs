using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sunduk.PWA.Infrastructure.Templates;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public abstract class TappingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public TappingTool Tool { get; set; }
        public double CutSpeed { get; set; }
        public double StartZ { get; set; }
        public double EndZ { get; set; }

        public override string Name
        {
            get
            {
                var name = "Резьба метчиком";
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

        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var fullLength = (Math.Abs(EndZ) + Math.Abs(StartZ));
                var feed = Tool.Pitch;
                var speed = CutSpeed;
                var spins = (speed * 1000) / (Math.PI * Tool.Diameter);
                if (spins > 3000) spins = 3000;
                cuttingTime += 2 * fullLength.AxialTurningTime(spins, feed);
                rapidTime += 1;
                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public TappingSequence(Machine machine, TappingTool tool, double cutSpeed, double startZ, double endZ)
        {
            Machine = machine;
            Tool = tool;
            CutSpeed = cutSpeed;
            StartZ = startZ;
            EndZ = endZ;
        }
    }
}
