using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние ArcGeneralComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class ArcGeneralState
    {
        public string CenterX { get; set; } = string.Empty;
        public string CenterZ { get; set; } = string.Empty;
        public string RadiusSize { get; set; } = string.Empty;
        public string InsertRadius { get; set; } = string.Empty;
        public Direction ArcDirection { get; set; } = Direction.CW;

        public ArcGeneralComponent.AnchorKind StartKind { get; set; } = ArcGeneralComponent.AnchorKind.Face;
        public string StartAnchorValue { get; set; } = string.Empty;
        public bool RoundStart { get; set; }
        public string StartBlunt { get; set; } = "0";

        public ArcGeneralComponent.AnchorKind EndKind { get; set; } = ArcGeneralComponent.AnchorKind.Cylinder;
        public string EndAnchorValue { get; set; } = string.Empty;
        public bool RoundEnd { get; set; }
        public string EndBlunt { get; set; } = "0";
    }
}
