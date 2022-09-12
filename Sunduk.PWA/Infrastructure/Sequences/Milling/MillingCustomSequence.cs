using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Sequences.Milling
{
    public class MillingCustomSequence : CustomSequence
    {
        public Coolant Coolant { get; set; }
        public bool Polar { get; set; }
        public double SafePlane { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; }
        public override string Operation => Templates.Operation.MillingCustomOperation(Machine, CoordinateSystem, Tool, CustomOperation, Coolant, Polar, SafePlane);
        public override MachineType MachineType => MachineType.Milling;

        public MillingCustomSequence(Machine machine, CoordinateSystem coordinateSystem, Tool tool, string customOperation, Coolant coolant, bool polar, double safePlane)
            : base(machine, tool, customOperation)
        {
            CoordinateSystem = coordinateSystem;
            Coolant = coolant;
            Polar = polar;
            SafePlane = safePlane;
        }
    }
}
