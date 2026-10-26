using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Time;
using Sunduk.PWA.Infrastructure.Tools.Base;

namespace Sunduk.PWA.Infrastructure.Files
{
    /// <summary>Содержимое файла работы (.cndc) — настройки заготовки/детали и вся программа
    /// (переходы вместе с контурами и инструментами).</summary>
    public sealed class CndcDocument
    {
        public int Version { get; set; } = CndcFile.CurrentVersion;
        public string? MachineId { get; set; }
        public string? MachineName { get; set; }
        public CoordinateSystem CoordinateSystem { get; set; } = CoordinateSystem.G54;
        public Material WorkpieceMaterial { get; set; }

        public string? DetailNumber { get; set; }
        public string? DetailName { get; set; }
        public string? Author { get; set; }
        public int OperationNumber { get; set; }
        public string? DrawingVersion { get; set; }
        public int? SpindleLimit { get; set; }

        public string? WorkpieceExternalDiameter { get; set; }
        public string? WorkpieceInternalDiameter { get; set; }
        public string? WorkpieceLength { get; set; }
        public string? WorkpieceFacingAllowance { get; set; }
        public string? SafePlane { get; set; }

        /// <summary>Пишется/читается отдельно (см. <see cref="CndcFile"/>) — чтобы один непонятный
        /// переход не ронял загрузку всего файла.</summary>
        [JsonIgnore]
        public List<Sequence> Program { get; set; } = new();
    }

    public sealed record CndcReadResult(CndcDocument Document, List<string> Errors);

    /// <summary>
    /// Чтение/запись файла работы Sunducam (.cndc, JSON). Переходы (около 40 классов с
    /// конструкторами и вычисляемыми свойствами) сохраняются обобщённо — свойствами с сеттером и
    /// автосвойствами без сеттера (через backing-поле); вычисляемые (<c>Operation</c>, время и т.п.)
    /// не пишутся, они считаются заново из сохранённых данных. Станок в переходах — ссылка по Id,
    /// инструменты — по значению (как в <see cref="Machine"/>).
    /// </summary>
    public static class CndcFile
    {
        public const int CurrentVersion = 1;
        public const string Extension = ".cndc";

        private static readonly Dictionary<string, Type> SequenceTypes = typeof(Sequence).Assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Sequence)) && !t.IsAbstract)
            .ToDictionary(t => t.Name);

        private static JsonSerializerOptions CreateOptions(Func<string?, Machine?> resolveMachine) => new()
        {
            WriteIndented = true,
            IncludeFields = true, // кортежи вроде (int, int) SeqNumbers — это поля
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Converters =
            {
                new JsonStringEnumConverter(),
                new MachineRefConverter(resolveMachine),
                new SequenceConverter(),
            },
        };

        public static string Write(CndcDocument document)
        {
            var options = CreateOptions(_ => null);
            var root = JsonSerializer.SerializeToNode(document, options)!.AsObject();
            var program = new System.Text.Json.Nodes.JsonArray();
            foreach (var sequence in document.Program)
                program.Add(JsonSerializer.SerializeToNode(sequence, typeof(Sequence), options));
            root["Program"] = program;
            return root.ToJsonString(options);
        }

        /// <summary><paramref name="resolveMachine"/> получает Id станка из файла (или null) и
        /// возвращает станок, который сейчас есть в приложении.</summary>
        public static CndcReadResult Read(string json, Func<string?, Machine?> resolveMachine)
        {
            var options = CreateOptions(resolveMachine);
            using var parsed = JsonDocument.Parse(json);
            var document = parsed.RootElement.Deserialize<CndcDocument>(options)
                ?? throw new JsonException("Пустой файл работы");
            if (document.Version > CurrentVersion)
                throw new JsonException($"Файл создан более новой версией (формат {document.Version}, поддерживается до {CurrentVersion})");

            var errors = new List<string>();
            if (parsed.RootElement.TryGetProperty("Program", out var program) && program.ValueKind == JsonValueKind.Array)
            {
                var index = 0;
                foreach (var item in program.EnumerateArray())
                {
                    index++;
                    try
                    {
                        var sequence = item.Deserialize<Sequence>(options);
                        if (sequence is not null) document.Program.Add(sequence);
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Переход {index}: {ex.Message}");
                    }
                }
            }
            return new CndcReadResult(document, errors);
        }

        /// <summary>Станок в переходе — только Id (сам станок живёт в реестре станков).</summary>
        private sealed class MachineRefConverter : JsonConverter<Machine>
        {
            private readonly Func<string?, Machine?> _resolve;
            public MachineRefConverter(Func<string?, Machine?> resolve) => _resolve = resolve;

            public override Machine? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => _resolve(reader.TokenType == JsonTokenType.Null ? null : reader.GetString());

            public override void Write(Utf8JsonWriter writer, Machine value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.Id);
        }

        private sealed record Member(string Name, Type Type, Func<object, object?> Get, Action<object, object?> Set)
        {
            /// <summary>Инструменты пишутся по базовому типу — только он несёт полиморфные атрибуты.</summary>
            public Type SerializedType => typeof(Tool).IsAssignableFrom(Type) ? typeof(Tool) : Type;
        }

        private static readonly ConcurrentDictionary<Type, Member[]> MemberCache = new();

        private static Member[] MembersOf(Type type) => MemberCache.GetOrAdd(type, t =>
        {
            var members = new List<Member>();
            foreach (var property in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetIndexParameters().Length > 0 || !property.CanRead) continue;
                if (property.PropertyType == typeof(OperationTime) || property.Name is "Operation" or "MachineTime") continue;

                var setter = property.GetSetMethod(nonPublic: true);
                if (setter is not null)
                {
                    members.Add(new Member(property.Name, property.PropertyType, o => property.GetValue(o), (o, v) => property.SetValue(o, v)));
                    continue;
                }
                // автосвойство без сеттера (заполняется в конструкторе) — через backing-поле;
                // вычисляемое (expression-bodied) поля не имеет и не сохраняется
                var field = property.DeclaringType?.GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field is not null)
                    members.Add(new Member(property.Name, property.PropertyType, o => property.GetValue(o), (o, v) => field.SetValue(o, v)));
            }
            return members.ToArray();
        });

        /// <summary>Переход ↔ JSON обобщённо: "$type" — имя класса, дальше его данные. Объект при
        /// чтении создаётся без конструктора (поля, не попавшие в файл, остаются по умолчанию).</summary>
        private sealed class SequenceConverter : JsonConverter<Sequence>
        {
            public override Sequence? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;
                var typeName = root.TryGetProperty("$type", out var t) ? t.GetString() : null;
                if (typeName is null || !SequenceTypes.TryGetValue(typeName, out var type))
                    throw new JsonException($"Неизвестный тип перехода «{typeName}»");

                var instance = RuntimeHelpers.GetUninitializedObject(type);
                foreach (var member in MembersOf(type))
                {
                    if (!root.TryGetProperty(member.Name, out var value)) continue;
                    member.Set(instance, value.Deserialize(member.SerializedType, options));
                }
                return (Sequence)instance;
            }

            public override void Write(Utf8JsonWriter writer, Sequence value, JsonSerializerOptions options)
            {
                var type = value.GetType();
                writer.WriteStartObject();
                writer.WriteString("$type", type.Name);
                foreach (var member in MembersOf(type))
                {
                    writer.WritePropertyName(member.Name);
                    JsonSerializer.Serialize(writer, member.Get(value), member.SerializedType, options);
                }
                writer.WriteEndObject();
            }
        }
    }
}
