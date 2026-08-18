using Sunduk.PWA.Components.TimeCalc;
using Sunduk.PWA.Infrastructure.CAM;
using Sunduk.PWA.Infrastructure.Sequences;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;
using System.Collections.Generic;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Состояние TurningTimeComponent на время жизни приложения (singleton в WASM).
    /// </summary>
    public class TurningTimeState
    {
        public TurningTimeComponent.TurningTypes TurningType { get; set; }
        public List<Sequence> Sequences { get; set; } = new();
        public Sequence CurrentSequence { get; set; }
        public Material Material { get; set; }

        public SequenceType SequenceType { get; set; }
        public CuttingType CuttingType { get; set; }
        public GeneralSequences GeneralSequence { get; set; }
        public TurningSequences TurningSequence { get; set; }
        public DrillingSequences DrillingSequence { get; set; }
        public ThreadingSequences ThreadingSequence { get; set; }
        public bool Tapping { get; set; } = true;

        public bool RadialGroove { get; set; } = true;
        public TurningGroovingSequences GroovingSequence { get; set; }

        public int Speed { get; set; }
        public double Feed { get; set; }
        public double StartZ { get; set; } = 2;
        public double EndZ { get; set; }
        public double StartX { get; set; }
        public double EndX { get; set; }

        public double StepOver { get; set; }
        public double Pitch { get; set; }
        public double PlaneLength { get; set; }
        public string CurrentThreadTemplate { get; set; }
        public int Steps { get; set; } = 1;

        public DrillingTool.Types DrillingToolType { get; set; }
        public ThreadStandard ThreadStandard { get; set; }
    }
}
