namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние CalculatorPage на время жизни приложения (singleton в WASM).
    /// InputMode/SetResultToExpression сюда не входят — они уже хранятся в LocalStorage.
    /// </summary>
    public class CalculatorPageState
    {
        public bool CalcSpins { get; set; } = true;
        public bool CalcMinFeed { get; set; } = true;

        public string DiameterString { get; set; } = string.Empty;
        public string CutSpeedString { get; set; } = string.Empty;
        public string SpindleSpeedString { get; set; } = string.Empty;
        public string EdgesString { get; set; } = string.Empty;
        public string FeedString { get; set; } = string.Empty;
        public string MinFeedString { get; set; } = string.Empty;

        public string Answer { get; set; } = string.Empty;
        public string UserExpression { get; set; } = string.Empty;
    }
}
