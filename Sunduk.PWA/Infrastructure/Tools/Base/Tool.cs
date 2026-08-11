using System.Text.Json.Serialization;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.PWA.Infrastructure.Tools.Base
{
    /// <summary>
    /// Полиморфная сериализация нужна т.к. Tool теперь хранится внутри Machine.Tools —
    /// один JSON-объект станка со списком инструментов разных конкретных типов.
    /// Trim-safe вариант (в отличие от reflection-based), важно для release-паблиша.
    /// </summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(MillingTool), "millingTool")]
    [JsonDerivedType(typeof(MillingThreadCuttingTool), "millingThreadCuttingTool")]
    [JsonDerivedType(typeof(MillingTappingTool), "millingTappingTool")]
    [JsonDerivedType(typeof(MillingSpecialTool), "millingSpecialTool")]
    [JsonDerivedType(typeof(MillingDrillingTool), "millingDrillingTool")]
    [JsonDerivedType(typeof(MillingChamferTool), "millingChamferTool")]
    [JsonDerivedType(typeof(MillingBoreTool), "millingBoreTool")]
    [JsonDerivedType(typeof(TurningExternalTool), "turningExternalTool")]
    [JsonDerivedType(typeof(GroovingExternalTool), "groovingExternalTool")]
    [JsonDerivedType(typeof(TurningDrillingTool), "turningDrillingTool")]
    [JsonDerivedType(typeof(TurningTappingTool), "turningTappingTool")]
    [JsonDerivedType(typeof(TurningInternalTool), "turningInternalTool")]
    [JsonDerivedType(typeof(TurningInternalBurnishingTool), "turningInternalBurnishingTool")]
    [JsonDerivedType(typeof(TurningExternalBurnishingTool), "turningExternalBurnishingTool")]
    [JsonDerivedType(typeof(ThreadingInternalTool), "threadingInternalTool")]
    [JsonDerivedType(typeof(ThreadingExternalTool), "threadingExternalTool")]
    [JsonDerivedType(typeof(TurningSpecialTool), "turningSpecialTool")]
    [JsonDerivedType(typeof(GroovingInternalTool), "groovingInternalTool")]
    [JsonDerivedType(typeof(GroovingFaceTool), "groovingFaceTool")]
    public abstract class Tool
    {
        public enum ToolHand { Right, Left }
        public int Position { get; set; }
        public virtual string Name { get; set; }
        public virtual ToolHand Hand { get; set; }

        public abstract MachineType MachineType { get; }

        /// <summary>
        /// Название и параметры инструмента (без номера и станко-специфичного обрамления) —
        /// то, что подставляется в плейсхолдер {TOOL} шаблона перехода станка
        /// (<see cref="Machine.TransitionTemplate"/>) и используется как есть в таблице инструментов.
        /// </summary>
        public abstract string CallDetails { get; }
    }
}
