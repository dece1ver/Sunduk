﻿using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Tools.Turning
{
    public sealed class TurningTappingTool : TappingTool
    {
        public override MachineType MachineType => MachineType.Turning;

        public TurningTappingTool(int position, Types type, double diameter, double pitch, ToolHand hand = ToolHand.Right)
            :base(position, type, diameter, pitch, hand)
        {
            Position = position;
            Type = type;
            Diameter = diameter;
            Pitch = pitch;
            Hand = hand;
        }
    }
}
