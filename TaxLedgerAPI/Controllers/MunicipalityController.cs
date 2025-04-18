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
    public class MunicipalityController : ControllerBase
    {
        private readonly IMunicipalityService municipalityService;

        public MunicipalityController(IMunicipalityService municipalityService)
        {
            this.municipalityService = municipalityService;
        }

        [HttpGet("/Municipality/GetAll")]
        public ActionResult<ResponseGeneric<IEnumerable<Municipality>>> Get()
        {
            var response = municipalityService.GetMunicipalities();

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPost("/Municipality/Add/{name}")]
        public ActionResult<Response> Add(string name)
        {
            var response = municipalityService.AddMunicipality(new Municipality { Name = name });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpPut("/Municipality/Update/{Id}/{name}")]
        public ActionResult<Response> Update(int Id, string name)
        {
            var response = municipalityService.UpdateMunicipality(new Municipality { Id = Id, Name = name });

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }

        [HttpDelete("/Municipality/Delete/{id}")]
        public ActionResult<Response> Delete(int id)
        {
            var response = municipalityService.DeleteMunicipality(id);

            return StatusCode(StatusCodeHelper.GetStatusCode(response.Message), response);
        }
    }
}
