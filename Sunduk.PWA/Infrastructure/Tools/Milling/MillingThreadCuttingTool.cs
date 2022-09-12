using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingThreadCuttingTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Name
        {
            get
            {
                var name = "REZBOFREZA ";
                name += ThreadStandard switch
                {
                    ThreadStandard.Metric => $"D{Diameter.NC()} P{Pitch.NC()}",
                    ThreadStandard.BSPP => $"G D{StandardTemplate} P{Pitch.NC()}",
                    ThreadStandard.Trapezoidal => $"TR D{Diameter.NC()} P{Pitch.NC()}",
                    ThreadStandard.NPT => $"K{StandardTemplate} P{Pitch.NC()}",
                    _ => string.Empty
                };
                return name;
            }
        }

        public double Diameter { get; set; }
        public ThreadStandard ThreadStandard { get; set; }
        public string StandardTemplate { get; set; }
        public double Pitch { get; set; }

        public MillingThreadCuttingTool(int position, double diameter, ThreadStandard threadStandard, double pitch, string standardTemplate = "")
        {
            Position = position;
            ThreadStandard = threadStandard;
            Diameter = diameter;
            StandardTemplate = standardTemplate;
            Pitch = pitch;
        }
    }
}
