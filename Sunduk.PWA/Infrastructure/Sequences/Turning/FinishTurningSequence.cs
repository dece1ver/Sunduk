using Sunduk.Geometry.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System.Linq;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishTurningSequence : TurningSequence
    {
        public override string Operation => Templates.Operation.ContourTurning(Machine, CoordinateSystem, Tool, Contour, SpeedFinish, FeedFinish, Coolant, TimeSpan.FromSeconds(MachineTime.FullTime));
        public override OperationTime MachineTime
        {
            get
            {
                if (Contour == null || Contour.Count < 2) return new OperationTime(0, 0);
                double cuttingTime = 0;
                double rapidTime = 5;
                var startX = Math.Abs(Contour[0].X ?? 0);
                var endX = Math.Abs(Contour[1].X ?? 0);
                var startZ = Math.Abs(Contour[0].Z ?? 0);
                var endZ = Math.Abs(Contour[1].Z ?? 0);
                var fullHeight = Math.Abs(startX - endX) / 2;
                var fullLength = startZ + endZ;
                var speed = SpeedFinish;
                var feed = FeedFinish;
                var spins = (speed * 1000) / (Math.PI * endX);
                if (spins > 3000) spins = 3000;
                cuttingTime += (fullLength + Templates.Operation.Escaping()).AxialTurningTime(spins, feed);
                cuttingTime += (fullHeight + Templates.Operation.Escaping()).AxialTurningTime(spins, feed);

                rapidTime += fullLength.AxialRapidTime();
                rapidTime += fullLength.AxialRapidTime();

                return new OperationTime(cuttingTime, rapidTime);
            }
        }
        public int SpeedFinish { get; set; }
        public double FeedFinish { get; set; }
        public override string Name
        {
            get
            {
                var name = Tool switch
                {
                    TurningExternalTool => $"Наружное чистовое точение Ø{this.Contour.LastOrDefault()?.X} с возвратом на Ø{this.Contour.FirstOrDefault()?.X} на глубине {this.Contour.LastOrDefault()?.Z} мм",
                    TurningInternalTool => $"Внутреннее чистовое точение Ø{this.Contour.LastOrDefault()?.X} с возвратом на Ø{this.Contour.FirstOrDefault()?.X} на глубине {this.Contour.LastOrDefault()?.Z} мм",
                    _ => $"Чистовое точение Ø{this.Contour.LastOrDefault()?.X} на глубину {this.Contour.LastOrDefault()?.Z} мм",
                };
                return name;
            }
        }

        public FinishTurningSequence(
            Machine machine, 
            Material material, 
            TurningTool tool, 
            List<Element> contour, 
            double stepOver, 
            double roughStockAllow, 
            double profStockAllow, 
            int speedFinish, 
            double feedFinish)
            : base(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow)
        {
            this.SpeedFinish = speedFinish;
            this.FeedFinish = feedFinish;
        }
    }
}
