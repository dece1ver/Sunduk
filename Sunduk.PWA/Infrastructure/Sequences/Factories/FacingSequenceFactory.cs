using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class FacingSequenceFactory
    {
        public static FacingSequence CreateFacing(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            double feedFinish,
            Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, speedFinish, feedRough, feedFinish) { Coolant = coolant };

        public static RoughFacingCycleSequence CreateRoughFacingCycle(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            double feedRough,
            Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, feedRough) { Coolant = coolant };

        public static RoughFacingSequence CreateRoughFacing(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            double feedRough,
            Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, externalDiameter, internalDiameter, roughStockAllow, profStockAllow, stepOver, seqNumbers, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedRough, feedRough) { Coolant = coolant };

        public static FinishFacingCycleSequence CreateFinishFacingCycle(
            TurningExternalTool tool,
            Sequence roughSequence,
            int speedFinish,
            double feedFinish)
            => new(tool, roughSequence, speedFinish, feedFinish);

        public static FinishFacingSequence CreateFinishFacing(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            double feedFinish,
            Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, externalDiameter, internalDiameter, profStockAllow, bluntType, bluntCustomAngle, bluntCustomRadius, cornerBlunt, speedFinish, feedFinish) { Coolant = coolant };
    }
}
