namespace M450.UnitTesting.App.Tests;

public sealed class PrivatkontoTests
{
    [Fact]
    public void Beziehe_InnerhalvDesMaximalbetrags_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, maximalbetrag: 200m);

        // Act
        konto.Beziehe(250m);

        // Assert
        Assert.Equal(-150m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_BisAufMaximalbetrag_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, maximalbetrag: 200m);

        // Act
        konto.Beziehe(300m);

        // Assert
        Assert.Equal(-200m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_UeberMaximalbetrag_WirftException()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, maximalbetrag: 200m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(301m));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(100m, konto.Guthaben);
    }

    [Fact]
    public void ZahleEin_MitPositivemBetrag_ErhoehtGuthaben()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 50m);

        // Act
        konto.ZahleEin(50m);

        // Assert
        Assert.Equal(100m, konto.Guthaben);
    }

    [Fact]
    public void Konstruktor_MitNegativemMaximalbetrag_WirftException()
    {
        // Act
        var exception = Record.Exception(() => new Privatkonto(-1m));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Beziehe_MitNichtPositivemBetrag_WirftException(decimal betrag)
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, maximalbetrag: 200m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(betrag));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    private static Privatkonto NeuesKonto(decimal startGuthaben = 0m, decimal maximalbetrag = 500m)
        => new Privatkonto(maximalbetrag, startGuthaben: startGuthaben);
}
