using Microsoft.AspNetCore.Mvc;
using EstancieroResponse;
using EstancieroService;
using EstancieroRequest;
using EstancieroEntities;

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
        [HttpGet("{numeroPartidaa}")]
        public IActionResult ObtenerPartidaActual(int numeroPartidaa)
        {
            var partidas = partidaService.ObtenerPartida();
            var partidaActual = partidas.FirstOrDefault(x => x.NumeroPartida == numeroPartidaa);
            if (partidaActual != null)
            {
                return Ok(new
                {
                    success = true,
                    data = partidaActual,
                    message = "Partida obtenida correctamente"
                }); ;
            }
            else
            {
                return NotFound("No se encontro partida");
            }
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
                return Ok(new
                {
                    success = resultado.Exito,
                    data = new { numeroPartida = resultado.Data },
                    message = resultado.Mensaje
                }); ;
            }
            return BadRequest(new
            {
                success = resultado.Exito,
                message = resultado.Mensaje
            });
        }

        //Cambiar el estado de la partida 
        /*
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
        */

        //Pausar una partida
        [HttpPut("PausarPartida/{numeroPartida}")]
        public IActionResult PausarPartida(int numeroPartida)
        {
            Resultado<int> resultado = partidaService.CambiarEstadoPartida(numeroPartida, EstadoActualPartida.Pausada); 
            if (resultado.tipoStatusCode == TipoStatusCode.Ok)
            {
                return Ok(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje
                });
            }
            else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje
                });
            }
            return BadRequest(new
            {
                success = resultado.Exito,
                message = resultado.Mensaje,
            });
        }

        //Continuar una partida pausada 
        [HttpPut("ContinuarPartida/{numeroPartida}")] 
        public IActionResult ContinuarPartida(int numeroPartida)
        {
            Resultado<int> resultado = partidaService.CambiarEstadoPartida(numeroPartida, EstadoActualPartida.EnJuego);
            if (resultado.tipoStatusCode == TipoStatusCode.Ok)
            {
                return Ok(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje,
                });
            } else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje,
                });
            }
            return BadRequest(new
            {
                success = resultado.Exito,
                message = resultado.Mensaje,
            });
        }

        //Suspender una partida 
        [HttpPut("SuspenderPartida/{numeroPartida}")] 
        public IActionResult SuspenderPartida(int numeroPartida)
        {
            Resultado<int> resultado = partidaService.CambiarEstadoPartida(numeroPartida, EstadoActualPartida.Suspendida); 
            if (resultado.tipoStatusCode == TipoStatusCode.Ok)
            {
                return Ok(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje,
                });
            } else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(new
                {
                    success = resultado.Exito,
                    message = resultado.Mensaje,
                });
            }
            return BadRequest(new
            {
                success = resultado.Exito,
                message = resultado.Mensaje,
            }); 
        }

        //Reanudar una partida 
        [HttpGet("ReanudarPartida/{numeroPartida}")]
        public IActionResult ReanudarPartida(int numeroPartida)
        {
            Resultado<int> resultado = partidaService.CambiarEstadoPartida(numeroPartida, EstadoActualPartida.EnJuego);
            if (resultado.tipoStatusCode == TipoStatusCode.Ok)
            {
                return Ok(resultado);
            }
            else if (resultado.tipoStatusCode == TipoStatusCode.NotFound)
            {
                return NotFound(resultado);
            }
            return BadRequest(resultado);
        }

        //Turno
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

        [HttpGet("ObtenerDetallesJugadores/{numeroPartida}")]
        public IActionResult ObtenerDetallesJugadores(int numeroPartida)
        {
            Resultado<DetalleUsuarioEntities> resultado = partidaService.ObtenerDetallesUsuarios(numeroPartida); 
            if (resultado.Exito)
            {
                return Ok(new
                {
                    success = resultado.Exito,
                    data = resultado.Data,
                    message = resultado.Mensaje
                });
            }
            return NotFound(new
            {
                success = resultado.Exito,
                data = resultado.Data,
                message = resultado.Mensaje,
            }); 
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
