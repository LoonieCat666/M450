namespace M450.UnitTesting.App;

public enum KontoStatus
{
    Standard,
    Vip
}

public class Konto : IKonto
{
    public string KontoNummer { get; } = Guid.NewGuid().ToString("N");
    public decimal Guthaben { get; protected set; }

    public decimal AktivZins { get; }
    public decimal PassivZins { get; }
    public decimal AufgelaufenerZins;
    public KontoStatus Status { get; }

    public Konto(decimal aktivZins, decimal passivZins, decimal startGuthaben = 0, KontoStatus status = KontoStatus.Standard)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(aktivZins);
        ArgumentOutOfRangeException.ThrowIfNegative(passivZins);
        AktivZins = aktivZins;
        PassivZins = passivZins;
        Guthaben = startGuthaben;
        Status = status;
    }

    public decimal ZahleEin(decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        Guthaben += betrag;
        return Guthaben;
    }

    public virtual decimal Beziehe(decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        Guthaben -= betrag;
        return Guthaben;
    }

    public decimal Transferiere(IKonto konto, decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        if (konto == this)
        {
            throw new ArgumentException("Quell- und Zielkonto dürfen nicht identisch sein.", nameof(konto));
        }

        Beziehe(betrag);
        konto.ZahleEin(betrag);
        return Guthaben;
    }

    public decimal SchreibeZinsGut(int anzTage)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(anzTage);

        decimal zinssatz = Guthaben switch
        {
            < 0 => PassivZins,
            < 10_000m => AktivZins,
            < 50_000m => AktivZins + 0.5m,
            _ => Status == KontoStatus.Vip ? AktivZins + 1.5m : AktivZins + 0.75m
        };

        AufgelaufenerZins += Guthaben * (zinssatz / 100) * anzTage / 360;
        return AufgelaufenerZins;
    }

    public void SchliesseKontoAb()
    {
        Guthaben += AufgelaufenerZins;
        AufgelaufenerZins = 0;
    }
}
