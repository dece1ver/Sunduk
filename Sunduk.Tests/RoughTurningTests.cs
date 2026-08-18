using Sunduk.Geometry.ContourElements;
using Sunduk.Geometry.ContourElements.Base;
using Sunduk.PWA.Infrastructure;
using Sunduk.PWA.Infrastructure.Templates;
using Sunduk.PWA.Infrastructure.Tools.Turning;

namespace Sunduk.Tests;

/// <summary>
/// Структурная сверка цикла G71 чернового точения: проверяем, что цикл эмитится, содержит
/// P/Q-строки профиля и что профиль использует скомпенсированный контур. Точные координаты и
/// диалект G71 сверяются с конкретным станком (как и прочие циклы программы).
/// </summary>
public class RoughTurningTests
{
    [Fact]
    public void RoughTurning_EmitsG71CycleWithProfileBlock()
    {
        var machine = new Machine { MachineType = MachineType.Turning };
        var tool = new TurningExternalTool(1, TurningExternalTool.Types.Bar, 80, 0.8);
        var contour = new List<Element>
        {
            new Point(40, 0),
            new Point(20, -10),
            new Point(20, -20),
        };

        var gcode = Operation.RoughTurning(
            machine,
            CoordinateSystem.G54,
            tool,
            contour,
            stepOver: 1,
            roughStockAllow: 2,
            profStockAllow: 0.2,
            seqNo: (1, 2),
            speedRough: 200,
            feedRough: 0.15,
            coolant: Coolant.None);

        Assert.Contains("G71 U1", gcode);
        Assert.Contains("G71 P1 Q2", gcode);
        Assert.Contains("N1 ", gcode);
        Assert.Contains("N2 ", gcode);
        Assert.Contains("G1 X", gcode);
    }
}
