using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public class ThreadingInternalTool : ThreadingTool
    {
        public double Diameter { get; set; }

        public override MachineType MachineType { get => MachineType.Turning; }

        public ThreadingInternalTool(int position, double diameter, double pitch, double angle, ToolHand hand = ToolHand.Rigth)
            : base(position, pitch, angle, hand)
        {
            Diameter = diameter;
        }
    }
}
