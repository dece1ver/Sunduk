using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class DetailModel
    {
        public Materials Material { get; set; }
        public double ExternalDiameter { get; set; }
        public double InternalDiameter { get; set; } = 0;
        public double Length { get; set; }
    }
}
