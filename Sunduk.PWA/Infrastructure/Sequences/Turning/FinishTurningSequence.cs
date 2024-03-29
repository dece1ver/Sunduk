﻿using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using System.Linq;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishTurningSequence : TurningSequence
    {
        public override string Operation => string.Empty;
        public override OperationTime MachineTime => this.OperationTime();
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
