using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FacingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double RoughStockAllow { get; set; }
        public double ProfStockAllow { get; set; }
        public double StepOver { get; set; }
        public (int, int) SeqNumbers { get; set; }
        public Blunt BluntType { get; set; }
        public double BluntCustomAngle { get; set; }
        public double BluntCustomRadius { get; set; }
        public double CornerBlunt { get; set; }
        public int SpeedRough { get; set; }
        public int SpeedFinish { get; set; }
        public double FeedRough { get; set; }
        public double FeedFinish { get; set; }
        public override string Operation => Templates.FacingOperation.Facing(
            Machine, 
            Material, 
            Tool, 
            ExternalDiameter,
            Tool is null ? InternalDiameter : InternalDiameter - (Tool.Radius * 2),
            RoughStockAllow, 
            ProfStockAllow, 
            StepOver, 
            SeqNumbers,
            BluntType,
            BluntCustomAngle,
            BluntCustomRadius,
            CornerBlunt,
            true,
            true, 
            SpeedRough, 
            SpeedFinish, 
            FeedRough, 
            FeedFinish);

        public override MachineType MachineType => MachineType.Turning;
        public override string Name { get 
            {
                var name = ProfStockAllow > 0 ? $"Торцовка черновая с Ø{ExternalDiameter}" : $"Торцовка с Ø{ExternalDiameter}";
                if (InternalDiameter > 0) name += $" до Ø{InternalDiameter}";
                return name;
            } 
        }

        public FacingSequence(Machine machine, Material material, TurningExternalTool tool, double externalDiameter,
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
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            RoughStockAllow = roughStockAllow;
            ProfStockAllow = profStockAllow;
            StepOver = stepOver;
            SeqNumbers = seqNumbers;
            BluntType = bluntType;
            BluntCustomAngle = bluntCustomAngle;
            BluntCustomRadius = bluntCustomRadius;
            CornerBlunt = cornerBlunt;
            SpeedRough = speedRough is 0 ? Templates.Operation.CuttingSpeedRough(Material) : speedRough;
            SpeedFinish = speedFinish is 0 ? Templates.Operation.CuttingSpeedFinish(Material) : speedFinish;
            FeedRough = feedRough is 0 ? Templates.Operation.FeedRough(Tool.Radius) : feedRough;
            FeedFinish = feedFinish is 0 ? Templates.Operation.FeedFinish(Tool.Radius): feedFinish;

        }
    }
}
