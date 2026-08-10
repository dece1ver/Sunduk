namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние PointCoordinatesComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class PointCoordinatesState
    {
        public bool CalcPolar { get; set; }
        public double? X { get; set; }
        public double? Y { get; set; }
        public double? Radius { get; set; }
        public double? Angle { get; set; }
    }
}
