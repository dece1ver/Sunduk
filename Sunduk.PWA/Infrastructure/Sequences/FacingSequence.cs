using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class FacingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double RoughStockAllow { get; set; }
        public double ProfStockAllow { get; set; }
        public double StepOver { get; set; }
        public (int, int) SeqNumbers { get; set; }
        public override string Operation => Templates.Operation.Facing(Machine, Material, Tool, 
            ExternalDiameter, InternalDiameter - (Tool.Radius * 2), 
            RoughStockAllow, ProfStockAllow, StepOver, SeqNumbers);

        public FacingSequence(Machine machine, Material material, TurningExternalTool tool, double externalDiameter, double internalDiameter, 
            double roughStockAllow, double profStockAllow, double stepOver, (int, int) seqNumbers)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            RoughStockAllow = roughStockAllow;
            ProfStockAllow = profStockAllow;
            StepOver = stepOver;
            SeqNumbers = seqNumbers;
        }
    }
}
