using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning.Base
{
    public class TurningSequence : Sequence
    {
        public TurningSequence(Machine machine, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            Contour = contour;
            StepOver = stepOver;
            RoughStockAllow = roughStockAllow;
            ProfStockAllow = profStockAllow;
        }

        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningTool Tool { get; set; }
        public List<Element> Contour { get; set; }
        public double StepOver { get; set; }
        public double RoughStockAllow { get; set; }
        public double ProfStockAllow { get; set; }
        public override MachineType MachineType => MachineType.Turning;
    }
}
