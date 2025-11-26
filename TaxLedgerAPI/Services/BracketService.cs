using AutoMapper;
using DataEF;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;
using TaxLedgerAPI.Structs;

namespace TaxLedgerAPI.Services
{
    public class BracketService : IBracketService
    {
        private readonly ILogger<BracketService> logger;
        private readonly Context context;
        private readonly IMapper mapper;
        private readonly ICacheService cache;

        public BracketService(ILogger<BracketService> logger, Context context, IMapper mapper, ICacheService cache)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
            this.cache = cache;
        }

        public ResponseGeneric<IEnumerable<Bracket>> GetBrackets()
        {
            try
            {
                return new ResponseGeneric<IEnumerable<Bracket>>
                {
                    Data = cache.GetFromCacheByKey<IEnumerable<Bracket>>(nameof(Bracket)) ?? cache.AddToCacheAndReturn(nameof(Bracket), mapper.ProjectTo<Bracket>(context.Brackets).ToList()),
                    Success = true,
                    Message = MessageStruct.RetrieveSuccess
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return new ResponseGeneric<IEnumerable<Bracket>> { Success = false, Message = MessageStruct.RetrieveFailed };
            }
        }

        public Response AddBracket(Bracket bracket)
        {
            if (context.Brackets.Any(b => b.Category == bracket.Category))
            {
                return new Response { Success = false, Message = MessageStruct.EntityExist };
            }

            var newBracket = mapper.Map<DataEF.Models.Bracket>(bracket);

            context.Brackets.Add(newBracket);

            return TrySave();
        }

        public Response UpdateBracket(Bracket bracket)
        {
            var bracketEntity = context.Brackets.FirstOrDefault(b => b.Id == bracket.Id);

            if (bracketEntity == null)
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            bracketEntity.Category = bracket.Category;

            return TrySave();
        }

        public Response DeleteBracket(int id)
        {
            var bracket = context.Brackets.FirstOrDefault(b => b.Id == id);

            if (bracket == null)
            {
                return new Response { Success = false, Message = MessageStruct.EntityNotFound };
            }

            context.Brackets.Remove(bracket);

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
