using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class TurningCutOffSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public GroovingExternalTool Tool { get; set; }
        public double CuttingPoint { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double CornerBlunt { get; set; }
        public double StepOver { get; set; }
        public Blunt BluntType { get; set; }
        public double BluntCustomAngle { get; set; }
        public double BluntCustomRadius { get; set; }
        public int SpeedRough { get; set; }
        public double FeedRough { get; set; }

        public override MachineType MachineType => MachineType.Turning;
        public override string Operation => Templates.GroovingOperation.CutOffSequence(Machine, Tool, CuttingPoint, ExternalDiameter, InternalDiameter, CornerBlunt, StepOver, SpeedRough, FeedRough, BluntType, BluntCustomAngle, BluntCustomRadius);
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var startX = ExternalDiameter;
                var endX = InternalDiameter;
                var fullLength = (startX - endX) / 2;
                var steps = (int)Math.Round(fullLength / StepOver, MidpointRounding.ToPositiveInfinity);
                var speed = SpeedRough;
                var feed = FeedRough;
                var spins = (speed * 1000) / (Math.PI * ((startX + endX) / 2));
                if (spins > 3000) spins = 3000;
                cuttingTime += steps * (StepOver + Templates.Operation.Escaping()).AxialTurningTime(spins, feed);

                rapidTime += steps * (StepOver + Templates.Operation.Escaping()).AxialRapidTime();
                rapidTime += fullLength.AxialRapidTime();

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public override string Name => $"Отрезка";

        public TurningCutOffSequence(
            Machine machine, 
            Material material, 
            GroovingExternalTool tool, 
            double cuttingPoint, 
            double externalDiameter, 
            double internalDiameter, 
            double cornerBlunt, 
            double stepOver, 
            Blunt bluntType , 
            double bluntCustomAngle, 
            double bluntCustomRadius,
            int speedRough,
            double feedRough)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            CuttingPoint = cuttingPoint;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            CornerBlunt = cornerBlunt;
            StepOver = stepOver;
            BluntType = bluntType;
            BluntCustomAngle = bluntCustomAngle;
            BluntCustomRadius = bluntCustomRadius;
            SpeedRough = speedRough;
            FeedRough = feedRough;
        }
    }
}
