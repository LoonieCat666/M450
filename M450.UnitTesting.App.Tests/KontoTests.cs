namespace M450.UnitTesting.App.Tests;

public sealed class KontoTests
{
    [Fact]
    public void Konstruktor_ErstelltKontenMitEindeutigenNummern()
    {
        // Arrange

        // Act
        var erstesKonto = NeuesKonto();
        var zweitesKonto = NeuesKonto();

        // Assert
        Assert.NotEqual(erstesKonto.KontoNummer, zweitesKonto.KontoNummer);
    }

    [Fact]
    public void ZahleEin_MitPositivemBetrag_ErhoehtGuthaben()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        konto.ZahleEin(25m);

        // Assert
        Assert.Equal(125m, konto.Guthaben);
    }

    [Fact]
    public void Beziehe_MehrAlsGuthaben_ErlaubtUeberziehung()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        konto.Beziehe(125m);

        // Assert
        Assert.Equal(-25m, konto.Guthaben);
    }

    [Fact]
    public void Transferiere_MitPositivemBetrag_BelastetUndBeguenstigtKonten()
    {
        // Arrange
        var quellKonto = NeuesKonto(startGuthaben: 100m);
        var zielKonto = NeuesKonto(startGuthaben: 10m);

        // Act
        quellKonto.Transferiere(zielKonto, 40m);

        // Assert
        Assert.Equal(60m, quellKonto.Guthaben);
        Assert.Equal(50m, zielKonto.Guthaben);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void GeldOperationen_MitNichtPositivemBetrag_WerfenException(int betrag)
    {
        // Arrange
        var konto = NeuesKonto();
        var zielKonto = NeuesKonto();

        // Act
        var einzahlenException = Record.Exception(() => konto.ZahleEin(betrag));
        var beziehenException = Record.Exception(() => konto.Beziehe(betrag));
        var transferException = Record.Exception(
            () => konto.Transferiere(zielKonto, betrag));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(einzahlenException);
        Assert.IsType<ArgumentOutOfRangeException>(beziehenException);
        Assert.IsType<ArgumentOutOfRangeException>(transferException);
    }

    [Fact]
    public void Transferiere_AufDasselbeKonto_WirftExceptionUndAendertNichts()
    {
        // Arrange
        var konto = NeuesKonto(startGuthaben: 100m);

        // Act
        var exception = Record.Exception(() => konto.Transferiere(konto, 10m));

        // Assert
        Assert.IsType<ArgumentException>(exception);
        Assert.Equal(100m, konto.Guthaben);
    }

    [Fact]
    public void SchreibeZinsGut_BeiPositivemGuthaben_VerwendetAktivZins()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 1_000m);

        // Act
        konto.SchreibeZinsGut(30);

        // Assert
        Assert.Equal(3m, konto.AufgelaufenerZins);
        Assert.Equal(1_000m, konto.Guthaben);
    }

    [Theory]
    [InlineData(KontoStatus.Standard)]
    [InlineData(KontoStatus.Vip)]
    public void SchreibeZinsGut_BeiNegativemGuthaben_VerwendetPassivZinsUnabhaengigVomStatus(KontoStatus status)
    {
        // Arrange
        var konto = NeuesKonto(passivZins: 7.2m, startGuthaben: -1_000m, status: status);

        // Act
        konto.SchreibeZinsGut(30);

        // Assert
        Assert.Equal(-6m, konto.AufgelaufenerZins);
        Assert.Equal(-1_000m, konto.Guthaben);
    }

    [Fact]
    public void SchreibeZinsGut_Mehrmals_BerechnetKeinenZinseszins()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 1_000m);

        // Act
        konto.SchreibeZinsGut(30);
        konto.SchreibeZinsGut(30);

        // Assert
        Assert.Equal(6m, konto.AufgelaufenerZins);
        Assert.Equal(1_000m, konto.Guthaben);
    }

    [Fact]
    public void SchliesseKontoAb_VerbuchtenZinsUndSetztZinsspeicherZurueck()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 1_000m);
        konto.SchreibeZinsGut(30);

        // Act
        konto.SchliesseKontoAb();

        // Assert
        Assert.Equal(1_003m, konto.Guthaben);
        Assert.Equal(0m, konto.AufgelaufenerZins);
    }

    [Fact]
    public void Konstruktor_MitNegativemZinssatz_WirftException()
    {
        // Arrange

        // Act
        var aktivZinsException = Record.Exception(() => new Konto(-0.1m, 5m));
        var passivZinsException = Record.Exception(() => new Konto(1m, -0.1m));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(aktivZinsException);
        Assert.IsType<ArgumentOutOfRangeException>(passivZinsException);
    }

    [Fact]
    public void SchreibeZinsGut_MitNegativenTagen_WirftException()
    {
        // Arrange
        var konto = NeuesKonto();

        // Act
        var exception = Record.Exception(() => konto.SchreibeZinsGut(-1));

        // Assert
        Assert.IsType<ArgumentOutOfRangeException>(exception);
    }

    [Fact]
    public void Kontoabschluss_NachBuchungenImJahresverlauf_VerbuchtenGesamtzins()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, passivZins: 7.2m);

        // Act
        // 1. März: Konto eröffnen und CHF 1'000 einzahlen.
        konto.ZahleEin(1_000m);

        // 1. Juli: Zins für 120 Tage berechnen und CHF 1'000 einzahlen.
        konto.SchreibeZinsGut(120);
        konto.ZahleEin(1_000m);

        // 1. August: Zins für 30 Tage berechnen und CHF 3'000 beziehen.
        konto.SchreibeZinsGut(30);
        konto.Beziehe(3_000m);

        // 1. Oktober: Passivzins für 60 Tage berechnen und CHF 2'000 einzahlen.
        konto.SchreibeZinsGut(60);
        konto.ZahleEin(2_000m);

        // 31. Dezember: Zins für 90 Tage berechnen und das Konto abschliessen.
        konto.SchreibeZinsGut(90);
        konto.SchliesseKontoAb();

        // Assert
        // Buchungssaldo CHF 1'000 + Gesamtzins CHF 15 = CHF 1'015.
        Assert.Equal(1_015m, konto.Guthaben);
        Assert.Equal(0m, konto.AufgelaufenerZins);
    }

    [Theory]
    [InlineData(KontoStatus.Standard)]
    [InlineData(KontoStatus.Vip)]
    public void SchreibeZinsGut_GuthabenZwischen10000Und50000_ErhoehtZinsUmHalbesProzentUnabhaengigVomStatus(KontoStatus status)
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 20_000m, status: status);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(820m, konto.AufgelaufenerZins);
    }

    [Fact]
    public void SchreibeZinsGut_GuthabenAb50000_StatusVIP_ErhoehtZinsUmAnderthalbProzent()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 70_000m, status: KontoStatus.Vip);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(3_570m, konto.AufgelaufenerZins);
    }

    [Fact]
    public void SchreibeZinsGut_GuthabenAb50000_StatusStandard_ErhoehtZinsUmDreiviertelProzent()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 3.6m, startGuthaben: 70_000m, status: KontoStatus.Standard);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(3_045m, konto.AufgelaufenerZins);
    }

    [Theory]
    [InlineData(9_999.99, 0)]
    [InlineData(10_000.00, 50.00)]
    [InlineData(10_000.01, 50.00005)]
    public void SchreibeZinsGut_AmGrenzwert10000_WendetRichtigeZinsstufeAn(decimal startGuthaben, decimal erwarteterZins)
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 0m, startGuthaben: startGuthaben, status: KontoStatus.Standard);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(erwarteterZins, konto.AufgelaufenerZins);
    }

    [Theory]
    [InlineData(49_999.99, 249.99995)]
    [InlineData(50_000.00, 375.00)]
    [InlineData(50_000.01, 375.000075)]
    public void SchreibeZinsGut_AmGrenzwert50000_WendetRichtigeZinsstufeAn(decimal startGuthaben, decimal erwarteterZins)
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 0m, startGuthaben: startGuthaben, status: KontoStatus.Standard);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(erwarteterZins, konto.AufgelaufenerZins);
    }

    [Fact]
    public void SchreibeZinsGut_GuthabenUeber100000_VerwendetWeiterhinHoechsteZinsstufe()
    {
        // Arrange
        var konto = NeuesKonto(aktivZins: 0m, startGuthaben: 150_000m, status: KontoStatus.Vip);

        // Act
        konto.SchreibeZinsGut(360);

        // Assert
        Assert.Equal(2_250m, konto.AufgelaufenerZins);
    }

    [Fact]
    public void Konstruktor_OhneStatusangabe_SetztStatusAufStandard()
    {
        // Arrange

        // Act
        var konto = NeuesKonto();

        // Assert
        Assert.Equal(KontoStatus.Standard, konto.Status);
    }

    [Theory]
    [InlineData(KontoStatus.Standard)]
    [InlineData(KontoStatus.Vip)]
    public void Konstruktor_MitStatusangabe_SetztStatus(KontoStatus status)
    {
        // Arrange

        // Act
        var konto = NeuesKonto(status: status);

        // Assert
        Assert.Equal(status, konto.Status);
    }

    // Diese Hilfsmethode vermeidet wiederholte Konstruktoraufrufe mit denselben
    // Standardwerten. Einzelne Tests können benötigte Werte über benannte Parameter
    // gezielt ändern und ihr Arrange-Abschnitt bleibt kurz und gut lesbar.
    private static Konto NeuesKonto(
        decimal aktivZins = 1.5m,
        decimal passivZins = 5m,
        decimal startGuthaben = 0m,
        KontoStatus status = KontoStatus.Standard)
    {
        return new Konto(aktivZins, passivZins, startGuthaben, status);
    }
}
