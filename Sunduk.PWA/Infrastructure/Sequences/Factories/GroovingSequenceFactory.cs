using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Factories
{
    public static class GroovingSequenceFactory
    {
        public static TurningCutOffSequence CreateCutOff(
            Machine machine,
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
            double feedRough)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, cornerBlunt, stepOver, bluntType, bluntCustomAngle, bluntCustomRadius, speedRough, feedRough);

        public static TurningExternalGroovingSequence CreateExternalGrooving(
            Machine machine, Material material, GroovingExternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish);

        public static TurningInternalGroovingSequence CreateInternalGrooving(
            Machine machine, Material material, GroovingInternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish);

        public static TurningFaceGroovingSequence CreateFaceGrooving(
            Machine machine, Material material, GroovingFaceTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, int speedFinish, double feedRough, double feedFinish)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, speedFinish, feedRough, feedFinish);

        public static TurningExternalRoughGroovingSequence CreateExternalRoughGrooving(
            Machine machine, Material material, GroovingExternalTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, double feedRough)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, feedRough);

        public static TurningFaceRoughGroovingSequence CreateFaceRoughGrooving(
            Machine machine, Material material, GroovingFaceTool tool, double cuttingPoint, double externalDiameter, double internalDiameter, double width, double stepOver, double profStockAllow, double outerCornerBlunt, double innerCornerBlunt, Blunt outerBluntType, Blunt innerBluntType, int speedRough, double feedRough)
            => new(machine, material, tool, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, feedRough);
    }
}
