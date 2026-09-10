namespace M450.UnitTesting.App;

public class Sparkonto : Konto
{
    public Sparkonto(decimal aktivZins = 0, decimal passivZins = 0, decimal startGuthaben = 0)
        : base(aktivZins, passivZins, startGuthaben) { }

    public override decimal Beziehe(decimal betrag)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(betrag);
        if (betrag > Guthaben)
            throw new InvalidOperationException("Sparkonto kann nicht überzogen werden.");
        Guthaben -= betrag;
        return Guthaben;
    }
}
