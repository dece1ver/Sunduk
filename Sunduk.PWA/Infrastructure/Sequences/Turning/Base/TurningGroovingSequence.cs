using Sunduk.PWA.Infrastructure.Sequences.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning.Base
{
    public class TurningGroovingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public double CuttingPoint { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double Width { get; set; }
        public double StepOver { get; set; }
        public double OuterCornerBlunt { get; set; }
        public double InnerCornerBlunt { get; set; }
        public Blunt OuterBluntType { get; set; }
        public Blunt InnerBluntType { get; set; }
        public double ProfStockAllow { get; set; }
        public int SpeedRough { get; set; }
        public int SpeedFinish { get; set; }
        public double FeedRough { get; set; }
        public double FeedFinish { get; set; }

        public override MachineType MachineType => MachineType.Turning;

        public TurningGroovingSequence(
            Machine machine,
            Material material,
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
        {
            Machine = machine;
            Material = material;
            CuttingPoint = cuttingPoint;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            Width = width;
            StepOver = stepOver;
            ProfStockAllow = profStockAllow;
            OuterCornerBlunt = outerCornerBlunt;
            InnerCornerBlunt = innerCornerBlunt;
            OuterBluntType = outerBluntType;
            InnerBluntType = innerBluntType;
            SpeedRough = speedRough;
            SpeedFinish = speedFinish;
            FeedRough = feedRough;
            FeedFinish = feedFinish;
        }
    }
}
