using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Держит состояние страницы Sunducam на время жизни приложения (singleton в WASM),
    /// чтобы переход на другую страницу и обратно не сбрасывал текущую программу.
    /// </summary>
    public class SunducamState
    {
        public bool Initialized { get; set; }

        public Mode Mode { get; set; } = Mode.General;
        public int SelectedSequenceIndex { get; set; }
        public Sequence CurrentSelectedSequence { get; set; }
        public List<Sequence> Program { get; set; }

        public Machine Machine { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; } = CoordinateSystem.G54;
        public Material WorkpieceMaterial { get; set; }

        public string DetailNumber { get; set; }
        public string DetailName { get; set; }
        public string Author { get; set; }
        public int OperationNumber { get; set; }
        public string DrawingVersion { get; set; }
        public int? SpindleLimit { get; set; }

        public string WorkpieceExternalDiameter { get; set; } = "50";
        public string WorkpieceInternalDiameter { get; set; } = string.Empty;
        public string WorkpieceLength { get; set; } = "50";
        /// <summary>Припуск по торцу — сколько необработанного материала торчит за номинальным
        /// (чистовым) торцом Z0 в сторону +Z до начала обработки (то, что снимает торцовка).</summary>
        public string WorkpieceFacingAllowance { get; set; } = "2";
        public string SafePlane { get; set; } = "300";
    }
}
