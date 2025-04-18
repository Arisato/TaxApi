using AutoMapper;
using TaxLedgerAPI.Models;

namespace TaxLedgerAPI.Profiles
{
    public class BracketProfile : Profile
    {
        public BracketProfile()
        {
            CreateMap<DataEF.Models.Bracket, Bracket>().ReverseMap();
        }
    }
}
