using Sunduk.PWA.Infrastructure.Templates;
using Thread = Sunduk.PWA.Infrastructure.Templates.Thread;

namespace Sunduk.Tests;

/// <summary>
/// Сверка пределов готовой резьбы по ГОСТ 16093-2004 / ISO 965 (без поправки
/// на подъём витка) с опубликованными таблицами предельных отклонений.
/// Эталоны подтверждены вручную: M18×2.5-6g — 17.623…17.958,
/// M12×1.75-7H — 10.106…10.531, M6×1-8g — 5.694…5.974.
/// </summary>
public class FinishedThreadTests
{
    private const double D = 4;

    [Fact]
    public void M18x25_6g_External()
    {
        var r = Thread.GetGost16093(18, 2.5, Thread.ThreadPosition.g, 6);
        Assert.NotNull(r);
        Assert.Equal(-0.042, r!.Value.EsEi, D);
        Assert.Equal(0.335, r.Value.Tolerance, D);
        Assert.Equal(17.958, r.Value.Max, D);
        Assert.Equal(17.623, r.Value.Min, D);
    }

    [Fact]
    public void M6x1_8g_External()
    {
        var r = Thread.GetGost16093(6, 1.0, Thread.ThreadPosition.g, 8);
        Assert.NotNull(r);
        Assert.Equal(-0.026, r!.Value.EsEi, D);
        Assert.Equal(0.280, r.Value.Tolerance, D);
        Assert.Equal(5.974, r.Value.Max, D);
        Assert.Equal(5.694, r.Value.Min, D);
    }

    [Fact]
    public void M12x175_7H_Internal()
    {
        var r = Thread.GetGost16093(12, 1.75, Thread.ThreadPosition.H, 7);
        Assert.NotNull(r);
        Assert.Equal(0.0, r!.Value.EsEi, D);
        Assert.Equal(0.425, r.Value.Tolerance, D);
        Assert.Equal(10.106, r.Value.Min, D);
        Assert.Equal(10.531, r.Value.Max, D);
    }

    [Fact]
    public void External_GradeWithoutTd_ReturnsNull()
    {
        // Td определён только для степеней 4, 6 и 8.
        var r = Thread.GetGost16093(18, 2.5, Thread.ThreadPosition.g, 5);
        Assert.Null(r);
    }

    [Fact]
    public void Internal_GradeWithoutTd1_ReturnsNull()
    {
        // TD1 определён только для степеней 4–8.
        var r = Thread.GetGost16093(10, 1.5, Thread.ThreadPosition.H, 9);
        Assert.Null(r);
    }

    [Fact]
    public void Internal_FinePitchGrade7_ReturnsNull()
    {
        // Для степени 7 при P < 0.5 мм допуск TD1 в таблице отсутствует.
        var r = Thread.GetGost16093(6, 0.35, Thread.ThreadPosition.H, 7);
        Assert.Null(r);
    }

    [Fact]
    public void FieldCase_RoutesCalculation()
    {
        // Строчное поле — расчёт наружной резьбы (d), заглавное — внутренней (D1).
        var ext = Thread.GetGost16093(18, 2.5, Thread.ThreadPosition.h, 6);
        Assert.NotNull(ext);
        Assert.Equal(0.0, ext!.Value.EsEi, D);
        Assert.Equal(18.000, ext.Value.Max, D);
        Assert.Equal(17.665, ext.Value.Min, D);

        var int_ = Thread.GetGost16093(12, 1.75, Thread.ThreadPosition.G, 6);
        Assert.NotNull(int_);
        Assert.Equal(0.034, int_.Value.EsEi, D);
        Assert.Equal(10.140, int_.Value.Min, D);
        Assert.Equal(10.475, int_.Value.Max, D);
    }
}
