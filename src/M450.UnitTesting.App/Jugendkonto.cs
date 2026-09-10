namespace M450.UnitTesting.App;

public class Jugendkonto : Konto
{
    public decimal Bezugslimite { get; }

    public Jugendkonto(decimal bezugslimite, decimal aktivZins = 0, decimal passivZins = 0, decimal startGuthaben = 0)
        : base(aktivZins, passivZins, startGuthaben)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bezugslimite);
        Bezugslimite = bezugslimite;
    }

    public override decimal Beziehe(decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        if (betrag > Bezugslimite)
            throw new InvalidOperationException("Bezugslimite überschritten.");
        if (betrag > Guthaben)
            throw new InvalidOperationException("Jugendkonto kann nicht überzogen werden.");
        Guthaben -= betrag;
        return Guthaben;
    }
}
