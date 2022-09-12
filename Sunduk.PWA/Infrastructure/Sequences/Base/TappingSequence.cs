using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sunduk.PWA.Infrastructure.Templates;

namespace Sunduk.PWA.Infrastructure.Sequences.Base
{
    public class TappingSequence : Sequence
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

        public override OperationTime MachineTime => this.OperationTime();
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
