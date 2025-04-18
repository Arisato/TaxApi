using AutoMapper;
using DataEF;
using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;
using TaxLedgerAPI.Structs;

namespace TaxLedgerAPI.Services
{
    public class MunicipalityService : IMunicipalityService
    {
        private readonly ILogger<MunicipalityService> logger;
        private readonly Context context;
        private readonly IMapper mapper;

        public MunicipalityService(ILogger<MunicipalityService> logger, Context context, IMapper mapper) 
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public ResponseGeneric<IEnumerable<Municipality>> GetMunicipalities()
        {
            try
            {
                return new ResponseGeneric<IEnumerable<Municipality>>
                {
                    Data = mapper.ProjectTo<Municipality>(context.Municipalities),
                    Success = true,
                    Message = MessageStruct.RetrieveSuccess
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new ResponseGeneric<IEnumerable<Municipality>> { Success = false, Message = MessageStruct.RetrieveFailed };
            }
        }

        public Response AddMunicipality(Municipality municipality) 
        {
            if (context.Municipalities.Any(m => m.Name == municipality.Name)) 
            {
                return new Response { Success = false, Message = MessageStruct.EntityExist };
            }

            var newMunicipality = mapper.Map<DataEF.Models.Municipality>(municipality);

            context.Municipalities.Add(newMunicipality);

            return TrySave();
        }

        public Response UpdateMunicipality(Municipality municipality)
        {
            var municipalityEntity = context.Municipalities.FirstOrDefault(m => m.Id == municipality.Id);

            if (municipalityEntity == null)
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            municipalityEntity.Name = municipality.Name;

            return TrySave();
        }

        public Response DeleteMunicipality(int id)
        {
            var municipality = context.Municipalities.FirstOrDefault(m => m.Id == id);

            if (municipality == null) 
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            context.Municipalities.Remove(municipality);

            return TrySave();
        }

        private Response TrySave() 
        {
            try
            {
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new Response { Success = false, Message = MessageStruct.TransactionFailed };
            }

            return new Response { Success = true, Message = MessageStruct.TransactionSuccess };
        }
    }
}
