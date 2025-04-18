using AutoMapper;
using TaxLedgerAPI.Models;

namespace TaxLedgerAPI.Profiles
{
    public class LedgerProfile : Profile
    {
        public LedgerProfile()
        {
            CreateMap<DataEF.Models.Ledger, Ledger>().ReverseMap();
        }
    }
}
