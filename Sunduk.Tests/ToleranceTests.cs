using Sunduk.PWA.Infrastructure.Tolerances;

namespace Sunduk.Tests;

/// <summary>
/// Сверка расчёта допусков и посадок (ГОСТ 25346-89 / ISO 286) с опубликованными
/// таблицами предельных отклонений (ISO 286-2 / ГОСТ 25347). Значения помечены
/// комментарием с источником: сверено с таблицами RoyMech / Wikipedia «Engineering fit».
/// </summary>
public class ToleranceTests
{
    private static void AssertField(double nominal, string field, double upperUm, double lowerUm)
    {
        Assert.True(Tolerance.TryCalculate(nominal, field, out var r, out var err), err);
        Assert.Equal(upperUm, r.UpperDeviation, 4);
        Assert.Equal(lowerUm, r.LowerDeviation, 4);
    }

    [Fact]
    public void H7_Hole_50mm() => AssertField(50, "H7", 25, 0);

    [Fact]
    public void h7_Shaft_50mm() => AssertField(50, "h7", 0, -25);

    [Fact]
    public void f7_Shaft_50mm() => AssertField(50, "f7", -25, -50);

    [Fact]
    public void k6_Shaft_50mm() => AssertField(50, "k6", 18, 2);

    [Fact]
    public void K7_Hole_50mm() => AssertField(50, "K7", 7, -18);

    [Fact]
    public void N7_Hole_50mm() => AssertField(50, "N7", -8, -33);

    [Fact]
    public void P7_Hole_50mm() => AssertField(50, "P7", -17, -42);

    [Fact]
    public void S7_Hole_50mm() => AssertField(50, "S7", -34, -59);

    [Fact]
    public void js7_Shaft_50mm() => AssertField(50, "js7", 12.5, -12.5);

    [Fact]
    public void e8_Shaft_50mm() => AssertField(50, "e8", -50, -89);

    [Fact]
    public void H7_Hole_30mm() => AssertField(30, "H7", 21, 0);

    [Fact]
    public void k6_Shaft_30mm() => AssertField(30, "k6", 15, 2);

    [Fact]
    public void H8_Hole_100mm() => AssertField(100, "H8", 54, 0);

    [Fact]
    public void f7_Shaft_100mm() => AssertField(100, "f7", -36, -71);

    [Fact]
    public void H7_Hole_10mm() => AssertField(10, "H7", 15, 0);

    [Fact]
    public void h7_Shaft_3mm() => AssertField(3, "h7", 0, -10);

    [Fact]
    public void H7_Hole_450mm() => AssertField(450, "H7", 63, 0);

    [Fact]
    public void f7_Shaft_450mm() => AssertField(450, "f7", -68, -131);

    [Fact]
    public void u6_Shaft_50mm() => AssertField(50, "u6", 86, 70);

    [Fact]
    public void p6_Shaft_50mm() => AssertField(50, "p6", 42, 26);

    [Fact]
    public void Limits_And_Middle()
    {
        Assert.True(Tolerance.TryCalculate(50, "H7", out var r, out _));

        Assert.Equal(50.025, r.MaxSize, 4);
        Assert.Equal(50.000, r.MinSize, 4);
        Assert.Equal(12.5, r.MiddleDeviation, 4);
        Assert.Equal(50.0125, r.MiddleSize, 4);
        Assert.Equal(25, r.ToleranceValue, 4);
    }

    [Fact]
    public void Invalid_Field_Returns_False()
    {
        Assert.False(Tolerance.TryCalculate(50, "w7", out _, out _));
        Assert.False(Tolerance.TryCalculate(50, "H20", out _, out _));
        Assert.False(Tolerance.TryCalculate(0, "H7", out _, out _));
        Assert.False(Tolerance.TryCalculate(600, "H7", out _, out _));
    }
}
