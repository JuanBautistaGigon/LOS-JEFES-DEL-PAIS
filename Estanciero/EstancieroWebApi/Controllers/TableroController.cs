using EstancieroService;
using EstancieroResponse;
using EstancieroRequest;
using Microsoft.AspNetCore.Mvc;
namespace EstancieroWebApi.Controllers
{
    [Route("tablero")]
    [ApiController]
    public class TableroController : ControllerBase
    {
        private TableroService tableroService = new TableroService();


        [HttpPut]
        public IActionResult MoverJugador(int dni, int casillero)
        {
            tableroService.MoverJugador(dni, casillero);

            return Ok("jugador movido con exito");

        }

        [HttpPut ("xd")]
        public IActionResult ComprarPropiedad(int dni, int casillero)
        {
            tableroService.ComprarPropiedad(dni, casillero);
            return Ok();
        }
    }
}
