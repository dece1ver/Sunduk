using Blazored.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sunduk.PWA.Infrastructure.Tools.Base;
using Sunduk.PWA.Infrastructure.Tools.Milling;
using Sunduk.PWA.Infrastructure.Tools.Turning;
using static Sunduk.PWA.Infrastructure.Util;

namespace Sunduk.PWA.Infrastructure.State
{
    /// <summary>
    /// Список станков (встроенные + пользовательские) на время жизни приложения (singleton
    /// в WASM), персистится в LocalStorage. Заменяет собой прежний enum Machine.
    /// </summary>
    public class MachineRegistry
    {
        private const string StorageKey = "Machines";
        private const string CurrentMachineIdKey = "CurrentMachineId";
        private const string LegacyMachineKey = "Machine";

        private readonly ILocalStorageService _localStorage;

        public bool Initialized { get; private set; }
        public List<Machine> Machines { get; private set; } = new();

        public MachineRegistry(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public async Task<Machine> LoadAsync()
        {
            if (Initialized) return CurrentMachineFallback();

            var saved = await _localStorage.GetItemAsync<List<Machine>>(StorageKey);
            string? currentId;
            if (saved is { Count: > 0 })
            {
                Machines = saved;
                currentId = await _localStorage.GetItemAsync<string>(CurrentMachineIdKey);
                var migrated = MigrateCoordinateSystems(Machines);
                migrated |= MigrateBuiltInCoordinateSystems(Machines);
                migrated |= MigrateBuiltInTemplates(Machines);
                migrated |= MigrateHeaderTrailerTemplate(Machines);
                migrated |= MigrateTransitionTemplate(Machines);
                migrated |= MigrateHeaderTemplate(Machines);
                migrated |= await MigrateGlobalToolsAsync(Machines);
                if (migrated) await SaveAsync();
            }
            else
            {
                Machines = SeedBuiltInMachines();
                currentId = await MigrateLegacySelectionAsync();
                await SaveAsync();
            }

            Initialized = true;
            var current = Machines.FirstOrDefault(m => m.Id == currentId) ?? Machines[0];
            await SetCurrentAsync(current);
            return current;
        }

        public async Task SaveAsync()
        {
            await _localStorage.SetItemAsync(StorageKey, Machines);
        }

        public async Task SetCurrentAsync(Machine machine)
        {
            await _localStorage.SetItemAsync(CurrentMachineIdKey, machine.Id);
        }

        private Machine CurrentMachineFallback() => Machines.FirstOrDefault() ?? SeedBuiltInMachines()[0];

        /// <summary>
        /// Станки, сохранённые до появления Machine.CoordinateSystems, несут в JSON только
        /// старое поле DefaultCoordinateSystem (десериализуется в LegacyDefaultCoordinateSystem).
        /// Разово переносим его в новый список. Возвращает true, если что-то поменялось.
        /// </summary>
        private static bool MigrateCoordinateSystems(List<Machine> machines)
        {
            var migrated = false;
            foreach (var machine in machines)
            {
                if (machine.CoordinateSystems is { Count: > 0 }) continue;
                machine.CoordinateSystems = new() { machine.LegacyDefaultCoordinateSystem ?? CoordinateSystem.G54 };
                migrated = true;
            }
            return migrated;
        }

        /// <summary>
        /// Встроенные станки, сохранённые до того как у GS1500 в сиде стало 2 СК (или другого
        /// расширения сида в будущем), несут в JSON только старый неполный список. Разово
        /// довносим недостающие СК из актуального сида по Id — объединение, не замена: то, что
        /// пользователь мог сам добавить/убрать, не трогаем, только дополняем отсутствующее.
        /// Возвращает true, если что-то поменялось.
        /// </summary>
        private static bool MigrateBuiltInCoordinateSystems(List<Machine> machines)
        {
            var migrated = false;
            var seeds = SeedBuiltInMachines().ToDictionary(m => m.Id);
            foreach (var machine in machines)
            {
                if (!machine.IsBuiltIn || !seeds.TryGetValue(machine.Id, out var seed)) continue;
                foreach (var cs in seed.CoordinateSystems)
                {
                    if (machine.CoordinateSystems.Contains(cs)) continue;
                    machine.CoordinateSystems.Add(cs);
                    migrated = true;
                }
            }
            return migrated;
        }

        /// <summary>
        /// Встроенные станки, сохранённые до появления ToolCallTemplate/SafetyStringTemplate,
        /// несут в JSON пустые значения этих полей (старые ToolDescriptionOption/SafetyStringStyle
        /// молча пропали при десериализации). Разово подставляем актуальные шаблоны из
        /// SeedBuiltInMachines по Id, не трогая то, что пользователь мог заполнить сам.
        /// Возвращает true, если что-то поменялось.
        /// </summary>
        private static bool MigrateBuiltInTemplates(List<Machine> machines)
        {
            var migrated = false;
            var seeds = SeedBuiltInMachines().ToDictionary(m => m.Id);
            foreach (var machine in machines)
            {
                if (!machine.IsBuiltIn || !seeds.TryGetValue(machine.Id, out var seed)) continue;
                if (string.IsNullOrWhiteSpace(machine.ToolCallTemplate) && !string.IsNullOrWhiteSpace(seed.ToolCallTemplate))
                {
                    machine.ToolCallTemplate = seed.ToolCallTemplate;
                    migrated = true;
                }
                if (string.IsNullOrWhiteSpace(machine.SafetyStringTemplate) && !string.IsNullOrWhiteSpace(seed.SafetyStringTemplate))
                {
                    machine.SafetyStringTemplate = seed.SafetyStringTemplate;
                    migrated = true;
                }
                // Очень старые встроенные сохранения без ToolCallTemplate/HeaderTrailerTemplate
                // вообще (оба пусты) — без этой ветки MigrateTransitionTemplate/MigrateHeaderTemplate
                // ниже подставили бы общий "{T}" вместо реального шаблона станка (например с
                // хардкод-токенами GS1500 вроде "G54 M58").
                if (string.IsNullOrWhiteSpace(machine.TransitionTemplate) && string.IsNullOrWhiteSpace(machine.ToolCallTemplate) && !string.IsNullOrWhiteSpace(seed.TransitionTemplate))
                {
                    machine.TransitionTemplate = seed.TransitionTemplate;
                    migrated = true;
                }
                if (string.IsNullOrWhiteSpace(machine.HeaderTemplate) && string.IsNullOrWhiteSpace(machine.HeaderTrailerTemplate) && !string.IsNullOrWhiteSpace(seed.HeaderTemplate))
                {
                    machine.HeaderTemplate = seed.HeaderTemplate;
                    migrated = true;
                }
            }
            return migrated;
        }

        /// <summary>
        /// Станки, сохранённые до появления Machine.HeaderTrailerTemplate, несут в JSON пустое
        /// значение этого поля — раньше строка автора/даты в конце шапки была жёстко зашита по
        /// HeaderStyle (см. Templates/Operation.cs). Разово реконструируем эквивалентный шаблон
        /// из уже сохранённого HeaderStyle станка (для ЛЮБОГО станка, не только встроенного —
        /// иначе у пользовательских станков молча пропали бы автор/дата из шапки). Возвращает
        /// true, если что-то поменялось.
        /// </summary>
        private static bool MigrateHeaderTrailerTemplate(List<Machine> machines)
        {
            var migrated = false;
            foreach (var machine in machines)
            {
                if (!string.IsNullOrWhiteSpace(machine.HeaderTrailerTemplate)) continue;
                machine.HeaderTrailerTemplate = machine.HeaderStyle switch
                {
                    HeaderStyle.AngleBracketName => "{AUTHOR} {DATE}",
                    _ => "{AUTHOR}{DATE}",
                };
                migrated = true;
            }
            return migrated;
        }

        /// <summary>
        /// Станки, сохранённые до появления Machine.TransitionTemplate, несут его пустым — данные
        /// лежат в старом Machine.ToolCallTemplate. Переносим текст как есть; для ТОКАРНЫХ станков
        /// дописываем недостающие {CS}/{COOLANT}/{PROCESSING}/{COOLANT_OFF}/{MACHINE_TIME} —
        /// {CS}/{COOLANT} раньше гарантировала GCodeBuilder.CoordinateSystemFallback/CoolantOn
        /// (Contains-проверка и "запасной" вывод) для циклов с фиксированным телом; {PROCESSING}/
        /// {COOLANT_OFF} КРИТИЧНЫ для `TurningCustomOperation`/`ContourTurning` (Tier B) — без них
        /// тело обработки и выключение СОЖ в этих двух переходах вообще не попадут в УП, т.к. они
        /// используют этот же TransitionTemplate через Util.Transition, а не через Util.ToolCall
        /// (который эти три плейсхолдера принудительно игнорирует). Для ФРЕЗЕРНЫХ станков ничего
        /// не дописываем вообще — фрезерные переходы эту "запасную" логику никогда не вызывали (СК
        /// зашита в строку самого рабочего перемещения, СОЖ — в отдельную G43-строку с собственным
        /// условием на TransitionTemplateHasCoolant, тело — не через шаблон, MillingCustomOperation
        /// не использует Transition), и добавление любого из этих плейсхолдеров добавило бы новые
        /// строки, которых раньше не было, и сломало бы условие в G43-строке. Строки сами
        /// схлопнутся при неприменимости (одна СК у станка, Coolant.None) — см. Util.RenderTemplate.
        /// Возвращает true, если что-то поменялось.
        /// </summary>
        private static bool MigrateTransitionTemplate(List<Machine> machines)
        {
            var migrated = false;
            foreach (var machine in machines)
            {
                if (!string.IsNullOrWhiteSpace(machine.TransitionTemplate)) continue;
                var template = string.IsNullOrWhiteSpace(machine.ToolCallTemplate) ? "{T}" : machine.ToolCallTemplate;
                if (machine.MachineType == MachineType.Turning)
                {
                    if (!template.Contains("{CS}")) template += "\n{CS}";
                    if (!template.Contains("{COOLANT}")) template += "\n{COOLANT}";
                    if (!template.Contains("{PROCESSING}")) template += "\n{PROCESSING}{COOLANT_OFF}";
                    if (!template.Contains("{MACHINE_TIME}")) template += "\n{MACHINE_TIME}";
                }
                machine.TransitionTemplate = template;
                migrated = true;
            }
            return migrated;
        }

        /// <summary>
        /// Станки, сохранённые до появления Machine.HeaderTemplate, несут его пустым — данные
        /// лежат в HeaderTrailerTemplate (уже гарантировано непустое после
        /// MigrateHeaderTrailerTemplate выше). Переносим текст как есть и дописываем
        /// {MACHINE_TIME} — раньше время шапки было безусловной жёстко зашитой строкой
        /// (Templates.Operation.Header), теперь обычный плейсхолдер. Возвращает true, если
        /// что-то поменялось.
        /// </summary>
        private static bool MigrateHeaderTemplate(List<Machine> machines)
        {
            var migrated = false;
            foreach (var machine in machines)
            {
                if (!string.IsNullOrWhiteSpace(machine.HeaderTemplate)) continue;
                var trailer = machine.HeaderTrailerTemplate ?? string.Empty;
                machine.HeaderTemplate = (trailer.TrimEnd('\n') + "\n{MACHINE_TIME}").TrimStart('\n');
                migrated = true;
            }
            return migrated;
        }

        /// <summary>
        /// Инструменты раньше хранились в 19 плоских ключах LocalStorage, общих на всё
        /// приложение (см. старый Sunducam.razor до появления Machine.Tools). Разово раздаём их
        /// по станкам с пустым Tools (по MachineType — как и раньше, все токарные станки видели
        /// один и тот же набор), клонируя через JSON, чтобы у каждого станка были собственные
        /// экземпляры, а не общие ссылки с другим станком того же типа. Старые ключи после этого
        /// удаляются — миграция одноразовая, иначе новый пустой станок при следующей загрузке
        /// снова получил бы этот же набор. Возвращает true, если что-то поменялось.
        /// </summary>
        private async Task<bool> MigrateGlobalToolsAsync(List<Machine> machines)
        {
            var legacyTools = await LoadLegacyToolsAsync();

            var migrated = false;
            if (legacyTools.Count > 0)
            {
                foreach (var machine in machines.Where(m => m.Tools is not { Count: > 0 }))
                {
                    var matching = legacyTools.Where(t => t.MachineType == machine.MachineType).ToList();
                    if (matching.Count == 0) continue;
                    machine.Tools = JsonSerializer.Deserialize<List<Tool>>(JsonSerializer.Serialize(matching)) ?? new();
                    migrated = true;
                }
            }

            foreach (var key in LegacyToolKeys) await _localStorage.RemoveItemAsync(key);
            return migrated;
        }

        private static readonly string[] LegacyToolKeys =
        {
            "MillingBoreTools", "MillingChamferTools", "MillingDrillingTools", "MillingSpecialTools",
            "MillingTappingTools", "MillingThreadCuttingTools", "MillingTools",
            "GroovingExternalTools", "GroovingFaceTools", "GroovingInternalTools", "TurningSpecialTools",
            "ThreadingExternalTools", "ThreadingInternalTools", "TurningExternalBurnishingTools",
            "TurningInternalBurnishingTools", "TurningDrillingTools", "TurningExternalTools",
            "TurningInternalTools", "TurningTappingTools",
        };

        private async Task<List<Tool>> LoadLegacyToolsAsync()
        {
            var tools = new List<Tool>();
            async Task AddAsync<T>(string key) where T : Tool
            {
                var items = await _localStorage.GetItemAsync<List<T>>(key);
                if (items is { Count: > 0 }) tools.AddRange(items);
            }
            await AddAsync<MillingBoreTool>("MillingBoreTools");
            await AddAsync<MillingChamferTool>("MillingChamferTools");
            await AddAsync<MillingDrillingTool>("MillingDrillingTools");
            await AddAsync<MillingSpecialTool>("MillingSpecialTools");
            await AddAsync<MillingTappingTool>("MillingTappingTools");
            await AddAsync<MillingThreadCuttingTool>("MillingThreadCuttingTools");
            await AddAsync<MillingTool>("MillingTools");
            await AddAsync<GroovingExternalTool>("GroovingExternalTools");
            await AddAsync<GroovingFaceTool>("GroovingFaceTools");
            await AddAsync<GroovingInternalTool>("GroovingInternalTools");
            await AddAsync<TurningSpecialTool>("TurningSpecialTools");
            await AddAsync<ThreadingExternalTool>("ThreadingExternalTools");
            await AddAsync<ThreadingInternalTool>("ThreadingInternalTools");
            await AddAsync<TurningExternalBurnishingTool>("TurningExternalBurnishingTools");
            await AddAsync<TurningInternalBurnishingTool>("TurningInternalBurnishingTools");
            await AddAsync<TurningDrillingTool>("TurningDrillingTools");
            await AddAsync<TurningExternalTool>("TurningExternalTools");
            await AddAsync<TurningInternalTool>("TurningInternalTools");
            await AddAsync<TurningTappingTool>("TurningTappingTools");
            return tools;
        }

        /// <summary>
        /// Раньше Machine хранился как raw int (порядок enum: L230A=0, GS1500=1, A110=2) под
        /// ключом "Machine", без конвертера. Читаем этот ключ один раз при первой загрузке новой
        /// схемы, чтобы не сбросить выбор станка у существующих пользователей.
        /// </summary>
        private async Task<string?> MigrateLegacySelectionAsync()
        {
            try
            {
                var legacyIndex = await _localStorage.GetItemAsync<int?>(LegacyMachineKey);
                return legacyIndex switch
                {
                    0 => Machines[0].Id, // L230A
                    1 => Machines[1].Id, // GS1500
                    2 => Machines[2].Id, // A110
                    _ => null,
                };
            }
            catch
            {
                return null;
            }
        }

        private static List<Machine> SeedBuiltInMachines() => new()
        {
            new Machine
            {
                Id = "builtin-l230a",
                Name = "Hyundai L230A",
                MachineType = MachineType.Turning,
                IsBuiltIn = true,
                CoordinateSystems = new() { CoordinateSystem.G55 },
                TailstockOnCode = "M25",
                TailstockOffCode = "M28",
                SpindleClampCode = "M68",
                SpindleUnclampCode = "M69",
                CoolantOnCode = "M8",
                CoolantOffCode = "M9",
                LeadingReferentPoint = false,
                TrailingReferentPoint = true,
                HeaderStyle = HeaderStyle.ONumber,
                HeaderTemplate = "{AUTHOR}{DATE}\n{MACHINE_TIME}",
                SafetyStringTemplate = "G30 U0\nG30 W0\nG40 G80 {CS}\nG50 S{S}\nG96 G23",
                SafetySpeedCap = 5000,
                SafetyDefaultSpeed = 3000,
                TransitionTemplate = "T{T4} ({TOOL})\n{CS}\n{COOLANT}\n{PROCESSING}{COOLANT_OFF}\n{MACHINE_TIME}",
                Tools = DefaultTools.Turning(),
            },
            new Machine
            {
                Id = "builtin-gs1500",
                Name = "Goodway GS-1500",
                MachineType = MachineType.Turning,
                IsBuiltIn = true,
                CoordinateSystems = new() { CoordinateSystem.G54, CoordinateSystem.G55 },
                TailstockOnCode = "M225",
                TailstockOffCode = "M226",
                SpindleClampCode = "M11",
                SpindleUnclampCode = "M10",
                CoolantOnCode = "M58",
                CoolantOffCode = "M59",
                LeadingReferentPoint = true,
                TrailingReferentPoint = true,
                HeaderStyle = HeaderStyle.AngleBracketName,
                HeaderTemplate = "{AUTHOR} {DATE}\n{MACHINE_TIME}",
                HeaderExtraLines = "G10 L2 P1 Z-100. B300. (G54)\nG10 L2 P2 Z400. (G55)",
                SafetyStringTemplate = "G30 U0\nG30 W0\nG55 G30 B0\nG40 G80\nG50 S{S}\nG96",
                SafetySpeedCap = 4000,
                SafetyDefaultSpeed = 3500,
                TransitionTemplate = "T{T4} G54 M58 ({TOOL})\n{CS}\n{COOLANT}\n{PROCESSING}{COOLANT_OFF}\n{MACHINE_TIME}",
                Tools = DefaultTools.Turning(),
            },
            new Machine
            {
                Id = "builtin-a110",
                Name = "Victor A110",
                MachineType = MachineType.Milling,
                IsBuiltIn = true,
                CoordinateSystems = new() { CoordinateSystem.G57 },
                CoolantOnCode = "M8",
                CoolantOffCode = "M9",
                CoolantThroughOnCode = "M50",
                CoolantThroughOffCode = "M51",
                CoolantFullOffCode = "M9 M51",
                CoolantBlowOnCode = "M57",
                CoolantBlowOffCode = "M59",
                HeaderStyle = HeaderStyle.ONumber,
                HeaderTemplate = "{AUTHOR}{DATE}\n{MACHINE_TIME}",
                TransitionTemplate = "T{T} M6 ({TOOL})",
                Tools = DefaultTools.Milling(),
            },
        };
    }
}
