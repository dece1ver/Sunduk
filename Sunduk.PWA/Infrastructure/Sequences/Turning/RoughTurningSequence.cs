using Sunduk.PWA.Infrastructure.Sequences.ContourElements.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning.Base;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MudBlazor;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences.Turning
{
    public class RoughTurningSequence : TurningSequence
    {
        public override string Operation => string.Empty;
        public override OperationTime MachineTime => this.OperationTime(Material);

        public override string Name
        {
            get
            {
                var name = Tool switch
                {
                    TurningExternalTool => $"Наружное черновое точение с Ø{this.Contour.FirstOrDefault().X} по Ø{this.Contour.LastOrDefault().X} на глубину {this.Contour.LastOrDefault().Z} мм",
                    TurningInternalTool => $"Внутреннее черновое точение с Ø{this.Contour.FirstOrDefault().X} по Ø{this.Contour.LastOrDefault().X} на глубину {this.Contour.LastOrDefault().Z} мм",
                    _ => "Черновое точение",
                };
                return name;
            }
        }

        public RoughTurningSequence(Machine machine, Material material, TurningTool tool, List<Element> contour, double stepOver, double roughStockAllow, double profStockAllow) 
            : base(machine, material, tool, contour, stepOver, roughStockAllow, profStockAllow)
        {
        }
    }
}
