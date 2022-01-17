using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class FinishFacingSequence : Sequence
    {
        public Machine Machine { get; set; }
        public Material Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double ProfStockAllow { get; set; }
        public override string Operation => Templates.Operation.FinishFacing(Machine, Material, Tool, ExternalDiameter, InternalDiameter - (Tool.Radius * 2), ProfStockAllow);
        public override MachineType MachineType { get => MachineType.Turning; }

        public FinishFacingSequence(Machine machine, Material material, TurningExternalTool tool, 
            double externalDiameter, double internalDiameter, double profStockAllow)
        {
            Machine = machine;
            Material = material;
            Tool = tool;
            ExternalDiameter = externalDiameter;
            InternalDiameter = internalDiameter;
            ProfStockAllow = profStockAllow;
        }
    }
}
