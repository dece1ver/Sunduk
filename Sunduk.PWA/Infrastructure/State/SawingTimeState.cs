using Sunduk.PWA.Components.TimeCalc;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние SawingTimeComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class SawingTimeState
    {
        public bool Debug { get; set; }
        public SawingTimeComponent.Form WorkpieceForm { get; set; } = SawingTimeComponent.Form.Circle;
        public SawingTimeComponent.Material WorkpieceMaterial { get; set; } = SawingTimeComponent.Material.Stainless;
        public SawingTimeComponent.TimeValue TimeModifier { get; set; } = SawingTimeComponent.TimeValue.Minute;

        public string WorkpieceExternalDiameter { get; set; } = string.Empty;
        public string WorkpieceInternalDiameter { get; set; } = string.Empty;
        public string WorkpieceWidth { get; set; } = string.Empty;
        public string WorkpieceHeight { get; set; } = string.Empty;
        public string WorkpieceLength { get; set; } = string.Empty;
        public string PartsCount { get; set; } = string.Empty;
    }
}
