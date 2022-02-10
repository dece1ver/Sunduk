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
        public double BluntCustomAngle { get; set; }
        public double BluntCustomRadius { get; set; }

        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.CutOffSequence(Machine, Material, Tool, CuttingPoint, ExternalDiameter, InternalDiameter, CornerBlunt, StepOver, BluntType, BluntCustomAngle, BluntCustomRadius);

        public TurningCutOffSequence(Machine machine = default, Material material = default, GroovingExternalTool tool = null, double cuttingPoint = 0, double externalDiameter = 0, double internalDiameter = 0, double cornerBlunt = 0, double stepOver = 0, Blunt bluntType = default, double bluntCustomAngle = 0, double bluntCustomRadius = 0)
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
            BluntCustomAngle = bluntCustomAngle;
            BluntCustomRadius = bluntCustomRadius;
        }
    }
}
