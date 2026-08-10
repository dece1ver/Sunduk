using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MudBlazor;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughTurningSequence : TurningSequence
    {
        public override string Operation => Templates.Operation.ContourTurning(Machine, CoordinateSystem, Tool, Contour, SpeedRough, FeedRough, Coolant);
        public override OperationTime MachineTime
        {
            get
            {
                double cuttingTime = 0;
                double rapidTime = 5;
                var startX = Math.Abs(Contour[0].X ?? 0);
                var endX = Math.Abs(Contour[1].X ?? 0);
                var startZ = Math.Abs(Contour[0].Z ?? 0);
                var endZ = Math.Abs(Contour[1].Z ?? 0);
                var fullLength = startZ + endZ;
                var speed = SpeedRough;
                var feed = FeedRough;
                int steps;
                if (Math.Abs(startX - endX) < 0.001)
                {
                    steps = 1;
                }
                else
                {
                    steps = startX > endX
                    ? (int)Math.Round((startX - endX) / 2 / StepOver, MidpointRounding.ToPositiveInfinity)
                    : (int)Math.Round((endX - startX) / 2 / StepOver, MidpointRounding.ToPositiveInfinity);
                }
                var spins = (speed * 1000) / (Math.PI * ((startX + endX) / 2));
                if (spins > 3000) spins = 3000;
                cuttingTime += steps * (fullLength + Templates.Operation.Escaping()).AxialTurningTime(spins, feed);

                rapidTime += steps * fullLength.AxialRapidTime();
                rapidTime += steps * 2 * (StepOver.AxialRapidTime()); // подъемы и опускания между проходами

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public int SpeedRough { get; set; }
        public double FeedRough { get; set; }
        public override string Name
        {
            get
            {
                var name = Tool switch
                {
                    TurningExternalTool => $"Наружное черновое точение с Ø{this.Contour.FirstOrDefault()?.X} по Ø{this.Contour.LastOrDefault()?.X} на глубину {this.Contour.LastOrDefault()?.Z} мм",
                    TurningInternalTool => $"Внутреннее черновое точение с Ø{this.Contour.FirstOrDefault()?.X} по Ø{this.Contour.LastOrDefault()?.X} на глубину {this.Contour.LastOrDefault()?.Z} мм",
                    _ => $"Черновое точение Ø{this.Contour.LastOrDefault()?.X} на глубину {this.Contour.LastOrDefault()?.Z} мм",
                };
                return name;
            }
        }

        public RoughTurningSequence(
            Machine machine, 
            Material material, 
            TurningTool tool, 
            List<Element> contour, 
            double stepOver, 
            double roughStockAllow, 
            double profStockAllow, 
            int speedRough,
            double feedRough) 
            : base(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow)
        {
            SpeedRough = speedRough;
            FeedRough = feedRough;
        }
    }
}
