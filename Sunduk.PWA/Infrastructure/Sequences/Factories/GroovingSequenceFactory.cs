using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class GroovingSequenceFactory
    {
        public static TurningCutOffSequence CreateCutOff(
            Machine machine,
            CoordinateSystem coordinateSystem,
            Material material,
            GroovingExternalTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double cornerBlunt,
            double stepOver,
            Blunt bluntType,
            double bluntCustomAngle,
            double bluntCustomRadius,
            int speedRough,
            double feedRough,
            Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, cornerBlunt, stepOver, bluntType, bluntCustomAngle, bluntCustomRadius, speedRough, feedRough) { Coolant = coolant };

        public static TurningExternalGroovingSequence CreateExternalGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingExternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish) { Coolant = coolant };

        public static TurningInternalGroovingSequence CreateInternalGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingInternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish) { Coolant = coolant };

        public static TurningFaceGroovingSequence CreateFaceGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingFaceTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish) { Coolant = coolant };

        public static TurningExternalRoughGroovingSequence CreateExternalRoughGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingExternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, double feedRough, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, feedRough) { Coolant = coolant };

        public static TurningInternalRoughGroovingSequence CreateInternalRoughGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingInternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, double feedRough, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, feedRough) { Coolant = coolant };

        public static TurningFaceRoughGroovingSequence CreateFaceRoughGrooving(
            Machine machine, CoordinateSystem coordinateSystem, Material material, GroovingFaceTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, double feedRough, Coolant coolant = Coolant.General)
            => new(machine, coordinateSystem, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, feedRough) { Coolant = coolant };
    }
}
