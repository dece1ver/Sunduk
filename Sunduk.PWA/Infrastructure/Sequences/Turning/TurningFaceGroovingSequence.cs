using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningFaceGroovingSequence : TurningGroovingSequence
    {
        public GroovingFaceTool Tool { get; set; }

        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.FaceGroovingSequence(
            Machine, 
            Material, 
            Tool, 
            Width, // используется как startPoint
            ExternalDiameter, 
            InternalDiameter, 
            CuttingPoint, 
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
        public override string Name => $"Канавка торцевая {Width.ToPrettyString()}мм на Ø{InternalDiameter.ToPrettyString()}-{ExternalDiameter.ToPrettyString()}";
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var startX = ExternalDiameter;
                var endX = InternalDiameter;
                var width = (ExternalDiameter - InternalDiameter) / 2;
                var length = Math.Abs(CuttingPoint) + Math.Abs(Width); // надо переделать, вместо ширины сделать конечный Z или что-то такое
                var stepsZ = (int)Math.Round(length / StepOver, MidpointRounding.ToPositiveInfinity);
                if (width < Tool.Width) width = Tool.Width;
                var stepsX = (int)Math.Round(width * 2 / Tool.Width, MidpointRounding.ToPositiveInfinity);
                var roughSpeed = SpeedRough;
                var roughFeed = FeedRough;
                var finishSpeed = SpeedFinish;
                var finishFeed = FeedFinish;
                var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
                var finishSpins = (finishSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
                if (roughSpins > 3000) roughSpins = 3000;
                if (finishSpins > 3000) finishSpins = 3000;
                cuttingTime += stepsX * (stepsZ * (StepOver + Templates.Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));
                cuttingTime += 2 * (length + Templates.Operation.Escaping()).AxialTurningTime(finishSpins, finishFeed);

                rapidTime += stepsX * (stepsZ * (StepOver + Templates.Operation.Escaping()).AxialRapidTime());
                rapidTime += 3 * (length + Templates.Operation.Escaping()).AxialRapidTime();
                rapidTime += length.AxialRapidTime(); // ???

                return new OperationTime(cuttingTime, rapidTime);
            }
        }

        public TurningFaceGroovingSequence(
            Machine machine,
            Material material,
            GroovingFaceTool tool,
            double cuttingPoint,
            double externalDiameter,
            double internalDiameter,
            double width, // используется как startPoint
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
            : base(machine,
                material,
                cuttingPoint,
                externalDiameter, 
                internalDiameter, 
                width, 
                stepOver, 
                profStockAllow, 
                outerCornerBlunt, 
                innerCornerBlunt,
                outerBluntType,
                innerBluntType, 
                speedRough,
                speedFinish,
                feedRough, 
                feedFinish)
        {
            Tool = tool;
        }
    }
}
