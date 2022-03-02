using Sunduk.PWA.Infrastructure.Tools.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Tools.Turning.Base
{
    public class TurningGroovingTool : Tool
    {

        public double Width { get; set; }
        public double CornerRadius { get; set; }
        public enum Point { Left, Right }
        public Point ZeroPoint { get; set; }

        public TurningGroovingTool(int position, double width, Point zeroPoint, ToolHand hand, double cornerRadius)
        {
            Position = position;
            Width = width;
            ZeroPoint = zeroPoint;
            Hand = hand;
            CornerRadius = cornerRadius;
        }
    }
}
