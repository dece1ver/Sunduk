using System;

namespace Sunduk.PWA.Infrastructure.Sequences
{
    public class Sequence
    {
        
        public Guid Id { get; }
        public string Name { get; set; }
        public string Operation { get; set; }

        public Sequence()
        {
            Id = Guid.NewGuid();
        }
    }
}
