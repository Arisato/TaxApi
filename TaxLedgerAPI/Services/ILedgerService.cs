using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;

namespace TaxLedgerAPI.Services
{
    public interface ILedgerService
    {
        ResponseGeneric<decimal> GetLedger(string municipalityName, DateOnly date);

        Response AddLedger(Ledger ledger);

        Response UpdateLedger(Ledger ledger);

        Response DeleteLedger(int id);
    }
}
