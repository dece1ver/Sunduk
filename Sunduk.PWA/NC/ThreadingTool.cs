using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.NC
{
    public class ThreadingTool : Tool
    {
        public ThreadingTool(int position, string name)
        {
            Position = position;
            Name = name;
            Usage = SequenceType.Threading;
        }
    }
}
