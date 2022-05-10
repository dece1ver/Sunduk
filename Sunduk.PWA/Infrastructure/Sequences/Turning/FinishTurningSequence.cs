using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class FinishTurningSequence : TurningSequence
    {
        public override string Operation { get => string.Empty; }

        public FinishTurningSequence(Machine machine, Material material, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow)
            : base(machine, material, contour, stepOver, roughStockAllow, profStockAllow)
        {
        }
    }
}
