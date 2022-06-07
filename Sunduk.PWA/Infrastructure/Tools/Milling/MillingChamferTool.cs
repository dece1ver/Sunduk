using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Milling
{
    public sealed class MillingChamferTool : Tool
    {
        public override MachineType MachineType => MachineType.Milling;
        public override string Name => "FASKA";
        public double Diameter { get; set; }
        public double Angle { get; set; }
        public double TipCompensation { get; set; }

        public MillingChamferTool(int position, double diameter, double angle, double tipCompensation = 0, ToolHand hand = ToolHand.Right)
        {
            Position = position;
            Diameter = diameter;
            Angle = angle;
            TipCompensation = tipCompensation;
            Hand = hand;
        }
    }
}
