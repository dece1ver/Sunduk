﻿using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;

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
        public override OperationTime MachineTime => this.OperationTime();

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
