﻿using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningExternalGroovingSequence : TurningGroovingSequence
    {
        public GroovingExternalTool Tool { get; set; }

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
            true);
    public override string Name => $"Канавка наружная {Width.NC(option: Util.NcDecimalPointOption.Without)}мм на Ø{ExternalDiameter.NC(option: Util.NcDecimalPointOption.Without)}";
    public override OperationTime MachineTime => this.OperationTime(Material);

    public TurningExternalGroovingSequence(
            Machine machine,
            Material material,
            GroovingExternalTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double width,
            double stepOver,
            double profStockAllow,
            double outerCornerBlunt,
            double innerCornerBlunt,
            Blunt outerBluntType,
            Blunt innerBluntType)
            : base(machine, material, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType)
        {
            Tool = tool;
        }
    }
}
