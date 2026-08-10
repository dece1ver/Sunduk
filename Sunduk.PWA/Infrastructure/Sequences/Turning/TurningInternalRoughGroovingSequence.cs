using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningInternalRoughGroovingSequence : TurningGroovingSequence
    {
        public GroovingInternalTool Tool { get; set; }
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var startX = ExternalDiameter;
                var endX = InternalDiameter;
                var fullLengthX = (startX - endX) / 2;
                var stepsX = (int)Math.Round(fullLengthX / StepOver, MidpointRounding.ToPositiveInfinity);
                var width = Width;
                if (width < Tool.Width) width = Tool.Width;
                var stepsZ = (int)Math.Round(width / Tool.Width, MidpointRounding.ToPositiveInfinity);
                var roughSpeed = SpeedRough;
                var roughFeed = FeedRough;

                var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));

                if (roughSpins > 3000) roughSpins = 3000;

                cuttingTime += stepsZ * (stepsX * (StepOver + Templates.Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));

                rapidTime += stepsZ * (stepsX * (StepOver + Templates.Operation.Escaping()).AxialRapidTime());
                rapidTime += 3 * (fullLengthX + Templates.Operation.Escaping()).AxialRapidTime();
                rapidTime += 2 * (Math.Abs(CuttingPoint) + Templates.Operation.Escaping()).AxialRapidTime();
                rapidTime += fullLengthX.AxialRapidTime(); // ???

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.GroovingSequence(
            Machine,
            CoordinateSystem,
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
            false, SpeedRough, 0, FeedRough, 0, Coolant);
        public override string Name => $"Канавка внутренняя {Width.ToPrettyString()}мм на Ø{ExternalDiameter.ToPrettyString()}";

        public TurningInternalRoughGroovingSequence(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            double feedRough)
            : base(machine, coordinateSystem, material, cuttingPoint, externalDiameter, internalDiameter, width, stepOver, profStockAllow, outerCornerBlunt, innerCornerBlunt, outerBluntType, innerBluntType, speedRough, 0, feedRough, 0)
        {
            Tool = tool;
        }
    }
}
