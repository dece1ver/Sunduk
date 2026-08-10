using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние ChamferComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class ChamferState
    {
        public bool CalcStartDiam { get; set; } = true;
        public bool ShowClassicChamfer { get; set; } = true;
        public ChamferComponent.Chamfer ChamferType { get; set; } = ChamferComponent.Chamfer.External;
        public ChamferComponent.Calculation CalculationType { get; set; } = ChamferComponent.Calculation.Simplified;
        public ChamferComponent.Images CurrentImage { get; set; } = ChamferComponent.Images.ExternalChamfer;

        public string ChamferEndDiameter { get; set; } = string.Empty;
        public string ChamferStartDiameter { get; set; } = string.Empty;
        public string Angle { get; set; } = string.Empty;
        public string ChamferSize { get; set; } = string.Empty;
        public string ChamferInsertRadius { get; set; } = string.Empty;

        public bool RoundCorners { get; set; }
        public string RoundCorner { get; set; } = "0.3";
    }
}
