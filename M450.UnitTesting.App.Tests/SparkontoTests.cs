namespace M450.UnitTesting.App.Tests;

public sealed class SparkontoTests
{
    [Fact]
    public void Beziehe_MitAusreichendemGuthaben_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 200m);

        // Act
        konto.Beziehe(150m);

        // Assert
        Assert.Equal(50m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_GenauerKontobetrag_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        konto.Beziehe(100m);

        // Assert
        Assert.Equal(0m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_MehrAlsGuthaben_WirftException()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(101m));

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
        konto.ZahleEin(75m);

        // Assert
        Assert.Equal(125m, konto.Guthaben);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Beziehe_MitNichtPositivemBetrag_WirftException(decimal betrag)
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(betrag));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    private static Sparkonto NeuesKonto(decimal startGuthaben = 0m) => new Sparkonto(startGuthaben: startGuthaben);
}
