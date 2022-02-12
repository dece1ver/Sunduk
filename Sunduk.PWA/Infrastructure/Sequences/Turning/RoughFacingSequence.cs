using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughFacingSequence : Sequence
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
        public Blunt BluntType { get; set; }
        public double BluntCustomAngle { get; set; }
        public double BluntCustomRadius { get; set; }
        public double CornerBlunt { get; set; }
        public override string Operation => Templates.FacingOperation.RoughFacing(
            Machine, 
            Material, 
            Tool, 
            ExternalDiameter, 
            InternalDiameter - (Tool.Radius * 2), 
            RoughStockAllow, 
            ProfStockAllow, 
            StepOver, 
            SeqNumbers,
            BluntType,
            BluntCustomAngle,
            BluntCustomRadius,
            CornerBlunt);
        public override MachineType MachineType => MachineType.Turning;

        public RoughFacingSequence(Machine machine, Material material, TurningExternalTool tool, double externalDiameter, double internalDiameter,
            double roughStockAllow, double profStockAllow, double stepOver, (int, int) seqNumbers, Blunt bluntType = default, double bluntCustomAngle = 0, double bluntCustomRadius = 0, double cornerBlunt = 0)
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
            BluntType = bluntType;
            BluntCustomAngle = bluntCustomAngle;
            BluntCustomRadius = bluntCustomRadius;
            CornerBlunt = cornerBlunt;
        }
    }
}
