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

        //Iniciar una partida 
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

        //Cambiar el estado de la partida
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

        //Turno 
        [HttpPut("{numPartida}")]
        public IActionResult LanzarDado(int numeroPartida)

        {
            MovimientoResponse movimiento = partidaService.TurnoSiguiente(numeroPartida);

            if (movimiento.Success)
            {
                return Ok(movimiento);
            }
            return BadRequest("ocurrio un error");
        }

        //Comprar propiedad
        [HttpPut ("{dniPropietarioNuevo}")]
        public IActionResult ComprarPropiedad(int dniPropietarioNuevo, int casilleroActual)
        {
            partidaService.ComprarPropiedad(dniPropietarioNuevo, casilleroActual);

            return Ok("propiedad comprada con exito");
        }
        
    }
}
