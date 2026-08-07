using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class FacingSequenceFactory
    {
        public static FacingSequence CreateFacing(
            Machine machine,
            Material material,
            TurningExternalTool tool,
            double externalDiameter,
            double internalDiameter,
            double roughStockAllow,
            double profStockAllow,
            double stepOver,
            (int, int) seqNumbers,
            Blunt bluntType,
            double bluntCustomAngle,
            double bluntCustomRadius,
            double cornerBlunt,
            int speedRough,
            int speedFinish,
            double feedRough,
            double feedFinish)
            => new(machine, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, speedFinish, feedRough, feedFinish);

        public static RoughFacingCycleSequence CreateRoughFacingCycle(
            Machine machine,
            Material material,
            TurningExternalTool tool,
            double externalDiameter,
            double internalDiameter,
            double roughStockAllow,
            double profStockAllow,
            double stepOver,
            (int, int) seqNumbers,
            Blunt bluntType,
            double bluntCustomAngle,
            double bluntCustomRadius,
            double cornerBlunt,
            int speedRough,
            double feedRough)
            => new(machine, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, feedRough);

        public static RoughFacingSequence CreateRoughFacing(
            Machine machine,
            Material material,
            TurningExternalTool tool,
            double externalDiameter,
            double internalDiameter,
            double roughStockAllow,
            double profStockAllow,
            double stepOver,
            (int, int) seqNumbers,
            Blunt bluntType,
            double bluntCustomAngle,
            double bluntCustomRadius,
            double cornerBlunt,
            int speedRough,
            double feedRough)
            => new(machine, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, feedRough);

        public static FinishFacingCycleSequence CreateFinishFacingCycle(
            TurningExternalTool tool,
            Sequence roughSequence,
            int speedFinish,
            double feedFinish)
            => new(tool, roughSequence, speedFinish, feedFinish);

        public static FinishFacingSequence CreateFinishFacing(
            Machine machine,
            Material material,
            TurningExternalTool tool,
            double externalDiameter,
            double internalDiameter,
            double profStockAllow,
            Blunt bluntType,
            double bluntCustomAngle,
            double bluntCustomRadius,
            double cornerBlunt,
            int speedFinish,
            double feedFinish)
            => new(machine, material, tool, externalDiameter, internalDiameter, profStockAllow, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedFinish, feedFinish);
    }
}
