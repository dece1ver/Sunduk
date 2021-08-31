using Sunduk.PWA.Infrastructure.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class FacingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Materials Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double RoughStockAllow { get; set; }
        public double ProfStockAllow { get; set; }
        public double StepOver { get; set; }
        public override string Operation => Templates.Operation.Facing(Machine, Material, Tool, ExternalDiameter, InternalDiameter, RoughStockAllow, ProfStockAllow, StepOver);

        public FacingSequence(Machines machine, Materials material, TurningExternalTool tool, double externalDiameter, double internalDiameter, double roughStockAllow, double profStockAllow, double stepOver)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            RoughStockAllow = roughStockAllow;
            ProfStockAllow = profStockAllow;
            StepOver = stepOver;
        }

    }
}
