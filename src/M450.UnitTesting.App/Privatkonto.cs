namespace M450.UnitTesting.App;

public class Privatkonto : Konto
{
    public decimal MaximalbetragUeberziehung { get; }

    public Privatkonto(decimal maximalbetragUeberziehung, decimal aktivZins = 0, decimal passivZins = 0, decimal startGuthaben = 0)
        : base(aktivZins, passivZins, startGuthaben)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(maximalbetragUeberziehung);
        MaximalbetragUeberziehung = maximalbetragUeberziehung;
    }

    public override decimal Beziehe(decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        if (Guthaben - betrag < -MaximalbetragUeberziehung)
            throw new InvalidOperationException("Maximalbetrag der Überziehung überschritten.");
        Guthaben -= betrag;
        return Guthaben;
    }
}
