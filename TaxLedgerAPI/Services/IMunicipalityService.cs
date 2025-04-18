using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;

namespace TaxLedgerAPI.Services
{
    public interface IMunicipalityService
    {
        ResponseGeneric<IEnumerable<Municipality>> GetMunicipalities();

        Response AddMunicipality(Municipality municipality);

        Response UpdateMunicipality(Municipality municipality);

        Response DeleteMunicipality(int id);
    }
}
