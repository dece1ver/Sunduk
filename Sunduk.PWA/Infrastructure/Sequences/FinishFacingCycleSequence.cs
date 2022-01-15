﻿using Sunduk.PWA.Infrastructure.Tools;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class FinishFacingCycleSequence : Sequence
    {
        public Sequence RoughSequence { get; set; }
        public TurningExternalTool Tool { get; set; }

        public override string Operation 
        {
            get
            {
                return RoughSequence switch
                {
                    RoughFacingSequence => Templates.Operation.FinishFacingCycleFromRoughFacing(Tool, RoughSequence as RoughFacingSequence),
                    RoughFacingCycleSequence => Templates.Operation.FinishFacingCycleFromRoughCycleFacing(Tool, RoughSequence as RoughFacingCycleSequence),
                    FacingSequence => Templates.Operation.FinishFacingCycleFromFacing(Tool, RoughSequence as FacingSequence),
                    _ => null,
                };
            }
            set { }
        }

        public FinishFacingCycleSequence(TurningExternalTool tool, Sequence roughSequence)
        {
            RoughSequence = roughSequence;
            Tool = tool;
        }
    }
}
