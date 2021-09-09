using Sunduk.PWA.Infrastructure.Tools;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class FinishFacingSequence : Sequence
    {
        public Machines Machine { get; set; }
        public Materials Material { get; set; }
        public TurningExternalTool Tool { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; }
        public double ProfStockAllow { get; set; }
        
        public override string Operation => Templates.Operation.FinishFacing(Machine, Material, Tool, ExternalDiameter, InternalDiameter - (Tool.Radius * 2), ProfStockAllow);

        public FinishFacingSequence(Machines machine, Materials material, TurningExternalTool tool, 
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
