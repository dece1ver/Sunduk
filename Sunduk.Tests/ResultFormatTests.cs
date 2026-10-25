using Sunduk.PWA.Infrastructure;
using Sunduk.PWA.Infrastructure.State;

// Тесты формата временно меняют статические настройки, поэтому классы не должны идти параллельно
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Sunduk.Tests;

/// <summary>
/// Формат дробных чисел в результатах: «0.000» (с хвостовыми нулями) и «0.###» (без них).
/// Статические форматы возвращаются в исходное состояние, чтобы не влиять на другие тесты.
/// </summary>
public class ResultFormatTests
{
    [Theory]
    [InlineData("0.000", 3, true)]
    [InlineData("0.###", 3, false)]
    [InlineData("0.0", 1, true)]
    [InlineData("0", 0, false)]
    public void TryParse_ReadsPattern(string pattern, int decimals, bool zeros)
    {
        Assert.True(ResultFormat.TryParse(pattern, out var format));
        Assert.Equal(decimals, format.Decimals);
        Assert.Equal(zeros, format.TrailingZeros);
        Assert.Equal(pattern, format.Pattern);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("0.0#")]
    [InlineData("0.0000000")]
    [InlineData("0.")]
    public void TryParse_RejectsBrokenPattern(string pattern) =>
        Assert.False(ResultFormat.TryParse(pattern, out _));

    [Theory]
    [InlineData("0.000", 0.2, "0.200")]
    [InlineData("0.###", 0.2, "0.2")]
    [InlineData("0.000", 1.23456, "1.235")]
    [InlineData("0.###", 1.23456, "1.235")]
    [InlineData("0.###", 5.0, "5")]
    [InlineData("0.000", 5.0, "5.000")]
    [InlineData("0.00", -0.0001, "0.00")]
    [InlineData("0.##", -0.0001, "0")]
    [InlineData("0", 12.6, "13")]
    public void Format_RoundsAndKeepsZerosAsRequested(string pattern, double value, string expected)
    {
        Assert.True(ResultFormat.TryParse(pattern, out var format));
        Assert.Equal(expected, format.Format(value));
    }

    [Fact]
    public void ToPrettyString_And_NC_FollowActiveFormat()
    {
        var general = ResultFormats.General;
        var cam = ResultFormats.Cam;
        var wasCam = ResultFormats.IsCam;
        try
        {
            ResultFormats.General = new ResultFormat(2, true);
            ResultFormats.Cam = new ResultFormat(4, false);

            ResultFormats.IsCam = false;
            Assert.Equal("0.20", 0.2.ToPrettyString());
            Assert.Equal("5.00", 5.0.NC(option: Util.NcDecimalPointOption.Without));

            ResultFormats.IsCam = true;
            Assert.Equal("0.2", 0.2.ToPrettyString());
            Assert.Equal("5.", 5.0.NC());
            Assert.Equal("5", 5.0.NC(option: Util.NcDecimalPointOption.Without));
        }
        finally
        {
            ResultFormats.General = general;
            ResultFormats.Cam = cam;
            ResultFormats.IsCam = wasCam;
        }
    }

    [Fact]
    public void ExplicitPrecision_OverridesDecimalsButNotZeroSetting()
    {
        var general = ResultFormats.General;
        try
        {
            ResultFormats.General = new ResultFormat(3, true);
            Assert.Equal("10.5", 10.5.ToPrettyString(1));
            Assert.Equal("10", 10.4.ToPrettyString(0));
        }
        finally
        {
            ResultFormats.General = general;
        }
    }
}
