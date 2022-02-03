using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningCutOffSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public GroovingExternalTool Tool { get; set; }
        public double CuttingPoint { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double CornerBlunt { get; set; }
        public double StepOver { get; set; }
        public Blunt BluntType { get; set; }
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.CutOffSequence(Machine, Material, Tool, CuttingPoint, ExternalDiameter, InternalDiameter, CornerBlunt, StepOver, BluntType);

        public TurningCutOffSequence(Machine machine = default, Material material = default, GroovingExternalTool tool = null, double cuttingPoint = 0, double externalDiameter = 0, double internalDiameter = 0, double cornerBlunt = 0, double stepOver = 0, Blunt bluntType = default)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            CuttingPoint = cuttingPoint;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            CornerBlunt = cornerBlunt;
            StepOver = stepOver;
            BluntType = bluntType;
        }
    }
}
