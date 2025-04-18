using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaxLedgerAPI.Helpers;
using TaxLedgerAPI.Models;
using TaxLedgerAPI.Models.Responses;
using TaxLedgerAPI.Services;

namespace TaxLedgerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class BracketController : ControllerBase
    {
        private readonly IBracketService bracketService;

        public BracketController(IBracketService bracketService)
        {
            this.bracketService = bracketService;
        }

        [HttpGet("/Bracket/GetAll")]
        public ActionResult<ResponseGeneric<IEnumerable<Bracket>>> Get()
        {
            var response = bracketService.GetBrackets();

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPost("/Bracket/Add/{category}")]
        public ActionResult<Response> Add(decimal category)
        {
            var response = bracketService.AddBracket(new Bracket { Category = category });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPut("/Bracket/Update/{Id}/{category}")]
        public ActionResult<Response> Update(int Id, decimal category)
        {
            var response = bracketService.UpdateBracket(new Bracket { Id = Id, Category = category });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpDelete("/Bracket/Delete/{id}")]
        public ActionResult<Response> Delete(int id)
        {
            var response = bracketService.DeleteBracket(id);

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }
    }
}
