using AutoMapper;
using DataEF;
using Microsoft.EntityFrameworkCore;
using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;
using TaxLedgerAPI.Structs;

namespace TaxLedgerAPI.Services
{
    public class LedgerService : ILedgerService
    {
        private readonly ILogger<LedgerService> logger;
        private readonly Context context;
        private readonly IMapper mapper;

        public LedgerService(ILogger<LedgerService> logger, Context context, IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        public ResponseGeneric<decimal> GetLedger(string municipalityName, DateOnly date)
        {
            try
            {
                var ledger = context.Ledgers
                .Include(l => l.Municipality)
                .Include(l => l.Bracket)
                .Where(l => l.Municipality.Name == municipalityName && l.StartDate <= date && l.EndDate >= date)
                .OrderBy(l => l.EndDate)
                .FirstOrDefault();

                if (ledger == null)
                {
                    return new ResponseGeneric<decimal> { Success = false, Message = MessageStruct.EntityNotFound};
                }

                return new ResponseGeneric<decimal> { Data = ledger.Bracket.Category, Success = true, Message = MessageStruct.RetrieveSuccess};
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new ResponseGeneric<decimal> { Success = false, Message = MessageStruct.RetrieveFailed};
            }
        }

        public Response AddLedger(Ledger ledger)
        {
            if (context.Ledgers.Any(l => l.MunicipalityId == ledger.MunicipalityId && l.BracketId == ledger.BracketId && l.StartDate == ledger.StartDate && l.EndDate == ledger.EndDate))
            {
                return new Response { Success = false, Message = MessageStruct.EntityExist };
            }

            var newLedger = mapper.Map<DataEF.Models.Ledger>(ledger);

            context.Ledgers.Add(newLedger);

            return TrySave();
        }

        public Response UpdateLedger(Ledger ledger)
        {
            var ledgerEntity = context.Ledgers.FirstOrDefault(l => l.Id == ledger.Id);

            if (ledgerEntity == null)
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            ledgerEntity.MunicipalityId = ledger.MunicipalityId == 0 ? ledgerEntity.MunicipalityId : ledger.MunicipalityId;
            ledgerEntity.BracketId = ledger.BracketId == 0 ? ledgerEntity.BracketId : ledger.BracketId;
            ledgerEntity.StartDate = ledger.StartDate;
            ledgerEntity.EndDate = ledger.EndDate;

            return TrySave();
        }

        public Response DeleteLedger(int id)
        {
            var ledger = context.Ledgers.FirstOrDefault(l => l.Id == id);

            if (ledger == null)
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            context.Ledgers.Remove(ledger);

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
