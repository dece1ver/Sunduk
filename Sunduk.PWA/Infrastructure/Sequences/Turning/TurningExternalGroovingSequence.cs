using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningExternalGroovingSequence : TurningGroovingSequence
    {
        public GroovingExternalTool Tool { get; set; }

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
            true,
            SpeedRough,
            SpeedFinish,
            FeedRough,
            FeedFinish,
            Coolant);
    public override string Name => $"Канавка наружная {Width.NC(option: Util.NcDecimalPointOption.Without)}мм на Ø{ExternalDiameter.NC(option: Util.NcDecimalPointOption.Without)}";
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
            var stepsZ = (int)Math.Round(width / (Tool.Width * 2), MidpointRounding.ToPositiveInfinity);
            var roughSpeed = SpeedRough;
            var roughFeed = FeedRough;
            var finishSpeed = SpeedFinish;
            var finishFeed = FeedFinish;
            var roughSpins = (roughSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            var finishSpins = (finishSpeed * 1000) / (Math.PI * ((startX + endX) / 2));
            if (roughSpins > 3000) roughSpins = 3000;
            if (finishSpins > 3000) finishSpins = 3000;
            cuttingTime += stepsZ * (stepsX * (StepOver + Templates.Operation.Escaping()).AxialTurningTime(roughSpins, roughFeed));
            cuttingTime += 2 * (fullLengthX + Templates.Operation.Escaping()).AxialTurningTime(finishSpins, finishFeed);

            rapidTime += stepsZ * (stepsX * (StepOver + Templates.Operation.Escaping()).AxialRapidTime());
            rapidTime += 3 * (fullLengthX + Templates.Operation.Escaping()).AxialRapidTime();
            rapidTime += fullLengthX.AxialRapidTime(); // ???

            return new OperationTime(cuttingTime, rapidTime);
        }
    }

    public TurningExternalGroovingSequence(
            Machine machine,
            CoordinateSystem coordinateSystem,
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
            Blunt innerBluntType,
            int speedRough,
            int speedFinish,
            double feedRough,
            double feedFinish)
            : base(
                machine,
                coordinateSystem,
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
