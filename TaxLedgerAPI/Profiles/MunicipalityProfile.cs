using AutoMapper;
using TaxLedgerAPI.Models;

namespace TaxLedgerAPI.Profiles
{
    public class MunicipalityProfile : Profile
    {
        public MunicipalityProfile() 
        {
            CreateMap<DataEF.Models.Municipality, Municipality>().ReverseMap();
        }
    }
}
