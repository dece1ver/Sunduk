using System.Text.Json.Serialization;

namespace Sunduk.Geometry.ContourElements.Base
{
    /// <summary>Узел контура. Полиморфная сериализация нужна для сохранения работы (.cndc):
    /// контур — список узлов разных типов.</summary>
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(Point), "point")]
    [JsonDerivedType(typeof(Line), "line")]
    [JsonDerivedType(typeof(Arc), "arc")]
    public abstract class Element
    {
        public abstract double? X { get; set; }
        public abstract double? Z { get; set; }
    }
}
