using Microsoft.AspNetCore.Mvc;
using TaxLedgerAPI.Helpers;
using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;
using TaxLedgerAPI.Services;

namespace TaxLedgerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class LedgerController : ControllerBase
    {
        private readonly ILedgerService ledgerService;

        public LedgerController(ILedgerService ledgerService)
        {
            this.ledgerService = ledgerService;
        }


        [HttpGet("/Ledger/Get/{municipalityName}/{date}")]
        public ActionResult<ResponseGeneric<decimal>> Get(string municipalityName, DateOnly date)
        {
            var response = ledgerService.GetLedger(municipalityName, date);

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPost("/Ledger/Add/{municipalityId}/{bracketId}/{startDate}/{endDate}")]
        public ActionResult<Response> Add(int municipalityId, int bracketId, DateOnly startDate, DateOnly endDate)
        {
            var response = ledgerService.AddLedger(new Ledger 
            { 
                MunicipalityId = municipalityId, 
                BracketId = bracketId, 
                StartDate = startDate, 
                EndDate = endDate 
            });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPut("/Ledger/Update/{ledgerId}/{municipalityId}/{bracketId}/{startDate}/{endDate}")]
        public ActionResult<Response> Update(int ledgerId, int municipalityId, int bracketId, DateOnly startDate, DateOnly endDate)
        {
            var response = ledgerService.UpdateLedger(new Ledger 
            { 
                Id = ledgerId, 
                MunicipalityId = municipalityId, 
                BracketId = bracketId, 
                StartDate = startDate, 
                EndDate = endDate 
            });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpDelete("/Ledger/Delete/{id}")]
        public ActionResult<Response> Delete(int id)
        {
            var response = ledgerService.DeleteLedger(id);

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }
    }
}
