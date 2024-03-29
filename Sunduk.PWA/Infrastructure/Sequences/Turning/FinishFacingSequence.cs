﻿using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishFacingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double ProfStockAllow { get; set; }
        public Blunt BluntType { get; set; }
        public double BluntCustomAngle { get; set; }
        public double BluntCustomRadius { get; set; }
        public double CornerBlunt { get; set; }
        public int SpeedFinish { get; set; }
        public double FeedFinish { get; set; }
        public override OperationTime MachineTime => this.OperationTime();
        public override string Operation => Templates.FacingOperation.FinishFacing(
            Machine, 
            Material, 
            Tool, 
            ExternalDiameter,
            Tool is null ? InternalDiameter : InternalDiameter - (Tool.Radius * 2),
            ProfStockAllow,
            BluntType,
            BluntCustomAngle,
            BluntCustomRadius,
            CornerBlunt, 
            SpeedFinish, 
            FeedFinish);
        public override MachineType MachineType => MachineType.Turning;
        public override string Name => $"Торцовка чистовая";

        public FinishFacingSequence(Machine machine, Material material, TurningExternalTool tool,
            double externalDiameter, 
            double internalDiameter, 
            double profStockAllow, 
            Blunt bluntType, 
            double bluntCustomAngle, 
            double bluntCustomRadius, 
            double cornerBlunt, 
            int speedFinish, 
            double feedFinish)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            ProfStockAllow = profStockAllow;
            BluntType = bluntType;
            BluntCustomAngle = bluntCustomAngle;
            BluntCustomRadius = bluntCustomRadius;
            CornerBlunt = cornerBlunt;
            SpeedFinish = speedFinish;
            FeedFinish = feedFinish;
        }
    }
}
