using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughTurningSequence : TurningSequence
    {
        public override string Operation => string.Empty;
        public override OperationTime MachineTime => this.OperationTime(Material);

        public RoughTurningSequence(Machine machine, Material material, Tool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow) 
            : base(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow)
        {
        }
    }
}
