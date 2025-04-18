using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;

namespace TaxLedgerAPI.Services
{
    public interface IBracketService
    {
        ResponseGeneric<IEnumerable<Bracket>> GetBrackets();

        Response AddBracket(Bracket bracket);

        Response UpdateBracket(Bracket bracket);

        Response DeleteBracket(int id);
    }
}
