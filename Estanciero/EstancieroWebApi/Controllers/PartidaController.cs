using Microsoft.AspNetCore.Mvc;
using EstancieroResponse;
using EstancieroService;
using EstancieroRequest;

namespace EstancieroWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PartidaController : ControllerBase
    {
        private PartidaService partidaService;

        public PartidaController()
        {
            partidaService = new PartidaService();
        }

        //Iniciar una partida *
        [HttpPost]
        public IActionResult DarDeAltaPartida([FromBody] PartidaRequest partidaRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Resultado<int> resultado = partidaService.CrearPartida(partidaRequest);

            if (resultado.Exito)
            {
                return Ok(resultado.Mensaje);
            }
            return BadRequest(resultado.Mensaje);
        }
        
        //Cambiar el estado de la partida
        [HttpPut("CambiarEstadoPartida/{numeroPartida}")]
        public IActionResult CambiarEstado(int numeroPartida, [FromQuery] EstadoActualPartida estadoNuevo)
        {
            Resultado<int> resultado = new Resultado<int>();
            if (estadoNuevo == EstadoActualPartida.Finalizada || estadoNuevo == EstadoActualPartida.Suspendida)
            {
                resultado = partidaService.FinalizarPartida(numeroPartida);
            }
            else
            {
                resultado = partidaService.CambiarEstadoPartida(numeroPartida, estadoNuevo);
            }

            if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(resultado.Mensaje);
            }
            if (resultado.tipoStatusCode == TipoStatusCode.BadRequest)
            {
                return BadRequest(resultado.Mensaje);
            }
            return Ok(resultado.Mensaje);
        }
        
        //Reanudar una partida *
        [HttpGet("ReanudarPartida/{numeroPartida2}")]
        public IActionResult ReanudarPartida(int numeroPartida)
        {
            Resultado<int> resultado = partidaService.CambiarEstadoPartida(numeroPartida, EstadoActualPartida.EnJuego);
            if (resultado.tipoStatusCode == TipoStatusCode.Ok)
            {
                return Ok(resultado);
            } else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(resultado);
            }
            return BadRequest(resultado);
        }

        //Turno *
        [HttpPut("LanzarDado/{numPartida}")]
        public IActionResult LanzarDado(int numPartida)
        {
            MovimientoResponse movimiento = partidaService.TurnoSiguiente(numPartida);

            if (movimiento.Success)
            {
                return Ok(movimiento);
            }
            return BadRequest("ocurrio un error");
        }

        /*Comprar propiedad
        [HttpPut("ComprarPropiedad/{dniPropietarioNuevo}")]
        public IActionResult ComprarPropiedad(int dniPropietarioNuevo, int casilleroActual)
        {
            partidaService.ComprarPropiedad(dniPropietarioNuevo, casilleroActual);

            return Ok("propiedad comprada con exito");
        }*/

    }
}
