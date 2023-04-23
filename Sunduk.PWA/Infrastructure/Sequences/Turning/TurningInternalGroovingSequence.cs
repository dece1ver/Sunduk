using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;

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
            InnerBluntType,
            true, 
            SpeedRough,
            SpeedFinish, 
            FeedRough, 
            FeedFinish);
        public override string Name => $"Канавка внутренняя {Width.ToPrettyString()}мм на Ø{ExternalDiameter.ToPrettyString()}";
        public override OperationTime MachineTime => this.OperationTime();

        public TurningInternalGroovingSequence(
            Machine machine,
            Material material,
            GroovingInternalTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double width,
            double stepOver,
            double profStockAllow,
            double outerCornerBlunt,
            double innerCornerBlunt,
            Blunt outerBluntType,
            Blunt innerBluntType, 
            int speedRough, 
            int speedFinish,
            double feedRough, 
            double feedFinish)
            : base(machine, material, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish)
        {
            Tool = tool;
        }
    }
}
