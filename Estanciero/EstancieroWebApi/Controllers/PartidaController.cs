using Microsoft.AspNetCore.Mvc;
using EstancieroResponse;
using EstancieroService;
using EstancieroRequest;
namespace EstancieroWebApi.Controllers
{
    [ApiController]
    [Route("controller")]
    public class PartidaController : ControllerBase
    {
        PartidaService partidaService = new PartidaService();

        [HttpPost]
        public IActionResult DarDeAltaPartida([FromBody] PartidaRequest partidaRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            partidaService.CrearPartida(partidaRequest);
            return Ok();
        }
        
    }
}
