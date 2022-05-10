using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughTurningSequence : TurningSequence
    {
        public override string Operation { get => string.Empty; }

        public RoughTurningSequence(Machine machine, Material material, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow) 
            : base(machine, material, contour, stepOver, roughStockAllow, profStockAllow)
        {
        }
    }
}
