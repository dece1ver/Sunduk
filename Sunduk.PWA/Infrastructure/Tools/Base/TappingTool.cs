using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sunduk.PWA.Infrastructure.Sequences;

namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    public class TappingTool : Tool
    {
        public enum Types { Forming, Cutting }

        public Types Type { get; set; }
        public ThreadStandard ThreadStandard { get; set; }
        public string StandardTemplate { get; set; }
        public double Diameter { get; set; }
        public double Pitch { get; set; }

        public override string Name
        {
            get
            {
                var name = Type switch
                {
                    Types.Forming => "RASKATNIK ",
                    Types.Cutting => "METCHIK ",
                    _ => string.Empty
                };

                name += ThreadStandard switch
                {
                    ThreadStandard.Metric => $"M{Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.BSPP => $"{StandardTemplate}",
                    ThreadStandard.Trapezoidal => $"Tr{Diameter.NC(option: Util.NcDecimalPointOption.Without)}x{Pitch.NC(option: Util.NcDecimalPointOption.Without)}",
                    ThreadStandard.NPT => $"{StandardTemplate}",
                    _ => string.Empty
                };
                return name;
            }
        }

        public TappingTool(int position, Types type, double diameter, double pitch, ThreadStandard threadStandard, string standardTemplate = "", ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            ThreadStandard = threadStandard;
            StandardTemplate = standardTemplate;
            Hand = hand;
        }
    }
}
