namespace M450.UnitTesting.App.Tests;

public sealed class JugendkontoTests
{
    [Fact]
    public void Beziehe_InnerhalbLimiteUndGuthaben_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 500m, bezugslimite: 100m);

        // Act
        konto.Beziehe(80m);

        // Assert
        Assert.Equal(420m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_GenauDieLimite_ErfolgreichAbbuchen()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 500m, bezugslimite: 100m);

        // Act
        konto.Beziehe(100m);

        // Assert
        Assert.Equal(400m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_UeberBezugslimite_WirftException()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 500m, bezugslimite: 100m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(101m));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(500m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_UeberGuthaben_WirftException()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 50m, bezugslimite: 100m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(75m));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(50m, konto.Guthaben);
    }

    [Fact]
    public void ZahleEin_MitPositivemBetrag_ErhoehtGuthaben()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, bezugslimite: 50m);

        // Act
        konto.ZahleEin(200m);

        // Assert
        Assert.Equal(300m, konto.Guthaben);
    }

    [Fact]
    public void Konstruktor_MitNichtPositiverBezugslimite_WirftException()
    {
        // Act
        var exception = Record.Exception(() => new Jugendkonto(0m));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Beziehe_MitNichtPositivemBetrag_WirftException(decimal betrag)
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m, bezugslimite: 50m);

        // Act
        var exception = Record.Exception(() => konto.Beziehe(betrag));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Transferiere_UeberBezugslimite_WirftExceptionUndAendertNichts()
    {
        // Arrange
        var quellKonto = NeuesKonto(startGuthaben: 500m, bezugslimite: 100m);
        var zielKonto = NeuesKonto(startGuthaben: 0m);

        // Act
        var exception = Record.Exception(() => quellKonto.Transferiere(zielKonto, 101m));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal(500m, quellKonto.Guthaben);
        Assert.Equal(0m, zielKonto.Guthaben);
    }

    private static Jugendkonto NeuesKonto(decimal startGuthaben = 0m, decimal bezugslimite = 200m)
        => new Jugendkonto(bezugslimite, startGuthaben: startGuthaben);
}
