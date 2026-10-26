using System.Runtime.CompilerServices;
using Sunduk.Geometry;
using Sunduk.Geometry.ContourElements;
using Sunduk.Geometry.ContourElements.Base;
using Sunduk.PWA.Infrastructure;
using Sunduk.PWA.Infrastructure.Files;
using Sunduk.PWA.Infrastructure.Sequences.Base;
using Sunduk.PWA.Infrastructure.Sequences.Turning;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.Tests;

/// <summary>Файл работы (.cndc): переход после записи и чтения должен давать тот же G-код.</summary>
public class CndcFileTests
{
    private static Machine NewMachine() => new() { MachineType = MachineType.Turning, Name = "Тест" };

    private static CndcDocument Document(Machine machine, params Sequence[] program) => new()
    {
        MachineId = machine.Id,
        MachineName = machine.Name,
        WorkpieceExternalDiameter = "50",
        WorkpieceLength = "60",
        DetailNumber = "AR-1",
        Program = program.ToList(),
    };

    private static CndcReadResult RoundTrip(CndcDocument document, Machine machine)
        => CndcFile.Read(CndcFile.Write(document), id => id == machine.Id ? machine : null);

    [Fact]
    public void TurningSequences_KeepContourToolAndGcode()
    {
        var machine = NewMachine();
        var tool = new TurningExternalTool(3, TurningExternalTool.Types.Bar, 80, 0.8);
        var contour = new List<Element>
        {
            new Point(16, 0, 2),
            new Line(20, -20, 0, 3),
            new Arc(30, -30, 5, Direction.CCW),
            new Line(30, -50),
        };
        var rough = new RoughTurningSequence(machine, Material.Steel, tool, contour, 1, 2, 0.2, 180, 0.2)
        {
            CoordinateSystem = CoordinateSystem.G54,
            SeqNumbers = (3, 4),
        };
        var finish = new FinishTurningSequence(machine, Material.Steel, tool, contour, 1, 2, 0.2, 220, 0.1);

        var result = RoundTrip(Document(machine, rough, finish, new StopSequence(true, "check")), machine);

        Assert.Empty(result.Errors);
        Assert.Equal(3, result.Document.Program.Count);
        var rough2 = Assert.IsType<RoughTurningSequence>(result.Document.Program[0]);
        Assert.Equal(rough.Operation, rough2.Operation);
        Assert.Equal((3, 4), rough2.SeqNumbers);
        Assert.Same(machine, rough2.Machine);
        Assert.Equal(4, rough2.Contour.Count);
        Assert.IsType<Arc>(rough2.Contour[2]);
        Assert.Equal(2, ((Point)rough2.Contour[0]).Blunt);
        Assert.Equal(finish.Operation, result.Document.Program[1].Operation);
        Assert.Equal("50", result.Document.WorkpieceExternalDiameter);
        Assert.Equal("AR-1", result.Document.DetailNumber);
    }

    [Fact]
    public void UnknownSequenceType_IsReportedAndOthersLoad()
    {
        var machine = NewMachine();
        var json = CndcFile.Write(Document(machine, new StopSequence(false, "a")))
            .Replace("\"Program\": [", "\"Program\": [{\"$type\":\"NoSuchSequence\"},");

        var result = CndcFile.Read(json, _ => machine);

        Assert.Single(result.Errors);
        Assert.Single(result.Document.Program);
    }

    [Fact]
    public void EveryConcreteSequenceType_SurvivesRoundTripWithDefaults()
    {
        // экземпляры без конструктора (все поля по умолчанию) — проверяем, что сериализатор
        // понимает типы свойств каждого из классов переходов, а не только токарных
        var machine = NewMachine();
        var types = typeof(Sequence).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Sequence)) && !t.IsAbstract);
        foreach (var type in types)
        {
            var sequence = (Sequence)RuntimeHelpers.GetUninitializedObject(type);
            var first = CndcFile.Write(Document(machine, sequence));
            var result = CndcFile.Read(first, _ => machine);
            Assert.True(result.Errors.Count == 0, $"{type.Name}: {string.Join("; ", result.Errors)}");
            Assert.Equal(first, CndcFile.Write(result.Document));
        }
    }
}
