using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Templates;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние ThreadTurningComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class ThreadTurningState
    {
        public string CurrentThreadTemplate { get; set; } = "";

        public ThreadStandard ThreadStandard { get; set; } = ThreadStandard.Metric;
        public CuttingType ThreadType { get; set; } = CuttingType.External;

        public double ThreadChamfer { get; set; }

        public string ThreadDiameterString { get; set; } = "";
        public string ThreadPitchString { get; set; } = "";

        public bool Blunted { get; set; }
        public double BluntSize { get; set; } = 0.3;

        public Thread.ThreadPosition? ThreadPosition { get; set; }
        public int? Grade { get; set; }

        public double PlaneLength { get; set; }

        public double StartPointZ { get; set; } = 2;
        public double EndPointZ { get; set; } = -20;
        public double InsertRadius { get; set; } = 0.4;
    }
}
