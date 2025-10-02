using Microsoft.AspNetCore.Mvc;
using EstancieroResponse;
using EstancieroService;
using EstancieroRequest;
using EstancieroEntities;
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
            Resultado<int> resultado = partidaService.CrearPartida(partidaRequest);
            return Ok(resultado.Mensaje);
        }

        [HttpPut("{numeroPartida}")]
        public IActionResult CambiarEstado(int numeroPartida, [FromBody] EstadoActualPartida estadoNuevo)
        {
            Resultado<int> resultado = new Resultado<int>();
            if (estadoNuevo == EstadoActualPartida.Finalizada || estadoNuevo == EstadoActualPartida.Suspendida)
            {
                resultado = partidaService.FinalizarPartida(numeroPartida);
                if (!resultado.Exito)
                {
                    return NotFound(resultado.Mensaje);
                }
                return Ok(resultado.Mensaje);
            }
             resultado = partidaService.CambiarEstadoPartida(numeroPartida, estadoNuevo);
            if (!resultado.Exito)
            {
                return NotFound(resultado.Mensaje);
            }
            return Ok(resultado.Mensaje);
        }


        [HttpPut("{dniJugador}")]
        public IActionResult LanzarDado(int dniJugador, int casilleroActual)

        {
            MovimientoResponse dado = partidaService.LanzarDado(dniJugador, casilleroActual);

            if (dado is not null)
            {
                return Ok($"el numero de dado que salio es {dado}");
            }
            return BadRequest("ocurrio un error");
        }

        [HttpPut ("{dniPropietarioNuevo}")]
        public IActionResult ComprarPropiedad(int dniPropietarioNuevo, int casilleroActual)
        {
            partidaService.ComprarPropiedad(dniPropietarioNuevo, casilleroActual);

            return Ok("propiedad comprada con exito");
        }
    }
}
