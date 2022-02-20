using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningExternalGroovingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public GroovingExternalTool Tool { get; set; }
        public double CuttingPoint { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double Width { get; set; }
        public double ExternalRadius { get; set; }
        public double InternalRadius { get; set; }
        public double StepOver { get; set; }
        public double ProfStockAllow { get; set; }
        public override MachineType MachineType => MachineType.Turning;
        public override string Name { get => $"Наружная канавка"; }

        public TurningExternalGroovingSequence(Machine machine, Material material, GroovingExternalTool tool, double externalDiameter, double internalDiameter, double width, double externalRadius, double internalRadius, double stepOver, double profStockAllow)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            Width = width;
            ExternalRadius = externalRadius;
            InternalRadius = internalRadius;
            StepOver = stepOver;
            ProfStockAllow = profStockAllow;
        }
    }
}
