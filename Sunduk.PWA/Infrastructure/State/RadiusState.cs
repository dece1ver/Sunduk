using Sunduk.PWA.Components.Stuff;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние RadiusComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class RadiusState
    {
        public bool ShowClassicRadius { get; set; } = true;
        public RadiusComponent.Images CurrentImage { get; set; } = RadiusComponent.Images.ExternalRadius;
        public RadiusComponent.Radius RadiusType { get; set; } = RadiusComponent.Radius.External;

        public string RadiusDiameter { get; set; } = string.Empty;
        public string RadiusSize { get; set; } = string.Empty;
        public string InsertRadius { get; set; } = string.Empty;
    }
}
