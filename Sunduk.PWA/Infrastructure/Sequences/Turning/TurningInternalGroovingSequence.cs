using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using Sunduk.PWA.Util;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningInternalGroovingSequence : TurningGroovingSequence
    {
        public GroovingInternalTool Tool { get; set; }

        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.GroovingSequence(
            Machine,
            Material,
            Tool,
            CuttingPoint,
            ExternalDiameter,
            InternalDiameter,
            Width,
            StepOver,
            ProfStockAllow,
            OuterCornerBlunt,
            InnerCornerBlunt,
            OuterBluntType,
            InnerBluntType);
        public override string Name => $"Канавка внутренняя {Width.ToPrettyString()}мм на Ø{ExternalDiameter.ToPrettyString()}";

        public TurningInternalGroovingSequence(
            Machine machine,
            Material material,
            GroovingInternalTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double width,
            double stepOver,
            double outerCornerBlunt,
            double innerCornerBlunt,
            Blunt outerBluntType,
            Blunt innerBluntType)
            : base(machine, material, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType)
        {
            Tool = tool;
        }
    }
}
