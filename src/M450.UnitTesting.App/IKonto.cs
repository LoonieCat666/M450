namespace M450.UnitTesting.App;

public interface IKonto
{
    string KontoNummer { get; }
    decimal Guthaben { get; }
    decimal ZahleEin(decimal betrag);
    decimal Beziehe(decimal betrag);
    decimal Transferiere(IKonto konto, decimal betrag);
}
