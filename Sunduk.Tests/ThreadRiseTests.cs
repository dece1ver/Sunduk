using Sunduk.PWA.Infrastructure.Templates;
using Thread = Sunduk.PWA.Infrastructure.Templates.Thread;

namespace Sunduk.Tests;

/// <summary>
/// Сверка величин подъёма витка для вязких материалов (ГОСТ 19258-73, рекомендуемое
/// приложение, табл. 1) и флага экстраполяции: таблица стандарта покрывает шаги
/// только от 0,2 до 2,0 мм включительно.
/// </summary>
public class ThreadRiseTests
{
    private const double D = 3;

    [Fact]
    public void TableBoundary_Inclusive()
    {
        // P = 2,0 — последняя строка табл. 1 приложения ГОСТ 19258-73.
        Assert.False(Thread.IsRiseExtrapolated(2.0));
        Assert.True(Thread.IsRiseExtrapolated(2.5));
        Assert.True(Thread.IsRiseExtrapolated(6.0));
    }

    [Fact]
    public void LastTableRow_MatchesGost()
    {
        // Табл. 1, P = 2,0: латунь 0,200 (наименьшее), коррозионностойкие 0,280 (наибольшее).
        var r = Thread.ThreadRise19258(2.0);
        Assert.Equal(0.200, r.Min, D);
        Assert.Equal(0.280, r.Max, D);
    }

    [Fact]
    public void TabularPoint_MatchesGost()
    {
        // Табл. 1, P = 1,5: латунь 0,160, коррозионностойкие 0,210.
        var r = Thread.ThreadRise19258(1.5);
        Assert.Equal(0.160, r.Min, D);
        Assert.Equal(0.210, r.Max, D);
    }

    [Fact]
    public void BeyondTable_IsExtrapolation()
    {
        // P > 2,0 в ГОСТ отсутствует: min заморожен на латуни P = 2,0 (0,200),
        // max линейно продолжает ряд коррозионностойких 0,140·P.
        var r = Thread.ThreadRise19258(2.5);
        Assert.Equal(0.200, r.Min, D);
        Assert.Equal(0.350, r.Max, D);
    }
}
