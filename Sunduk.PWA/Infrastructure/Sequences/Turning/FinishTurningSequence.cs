using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Time;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishTurningSequence : TurningSequence
    {
        public override string Operation => string.Empty;
        public override OperationTime MachineTime => this.OperationTime(Material);
        public override string Name => $"Чистовое точение";

        public FinishTurningSequence(Machine machine, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow)
            : base(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow)
        {
        }
    }
}
