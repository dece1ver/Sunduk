using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughFacingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
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
        public double FeedRough { get; set; }
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;

                var startX = ExternalDiameter;
                var endX = InternalDiameter;
                var startZ = RoughStockAllow;
                var endZ = ProfStockAllow;
                var speedRough = SpeedRough;
                var feedRough = FeedRough;
                var fullLength = startZ - endZ;
                var steps = (int)Math.Round(fullLength / StepOver, MidpointRounding.ToPositiveInfinity);
                var spins = (speedRough * 1000) / (Math.PI * ((startX - endX) / 2));
                if (spins > 3000) spins = 3000;
                cuttingTime += steps * Calc.CrossTurningTime(startX, endX, spins, feedRough);

                rapidTime += steps * Calc.CrossRapidTime(startX, endX);

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public override string Operation => Templates.FacingOperation.Facing(
            Machine,
            CoordinateSystem,
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
            false, 
            false, 
            SpeedRough, 
            0,
            FeedRough,
            0,
            Coolant);
        public override MachineType MachineType => MachineType.Turning;
        public override string Name { get 
                {
                var name = ProfStockAllow > 0 ? $"Торцовка черновая с Ø{ExternalDiameter}" : $"Торцовка с Ø{ExternalDiameter}";
                if (InternalDiameter > 0) name += $" до Ø{InternalDiameter}";
                return name;
                } 
            }

        public RoughFacingSequence(
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
            double feedRough)
        {
            Machine = machine;
            CoordinateSystem = coordinateSystem;
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
            SpeedRough = speedRough;
            FeedRough = feedRough;
        }
    }
}
