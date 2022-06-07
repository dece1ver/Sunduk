using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingThreadCuttingTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Name => "REZBOFREZA";

        public double Diameter { get; set; }
        public ThreadStandart ThreadStandart { get; set; }
        public double Pitch { get; set; }

        public MillingThreadCuttingTool(int position, double diameter, ThreadStandart threadStandart, double pitch)
        {
            Position = position;
            ThreadStandart = threadStandart;
            Diameter = diameter;
            Pitch = pitch;
        }
    }
}
