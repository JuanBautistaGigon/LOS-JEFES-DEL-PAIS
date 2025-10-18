using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstancieroData;
using EstancieroEntities;
using EstancieroResponse;
using EstancieroRequest;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace EstancieroService
{
    public class PartidaService
    {

        public Resultado<int> CrearPartida(PartidaRequest partidaRequest)
        {
            //Verificar que los usuarios esten registrados
            List<int> DnisJugadores = partidaRequest.Dnis;
            Resultado<int> resultado = VerificarUsuarios(DnisJugadores);
            if (!resultado.Exito)
            {
                return resultado;
            }
            /*List<UsuarioEntities> listaUsuariosRegistrados = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios")); 
            List<int> faltantes = partidaRequest.Dnis.Except(listaUsuariosRegistrados.Select(u => u.Dni)).ToList();
            if (faltantes.Any())                
            {
                return new Resultado<int> { Exito = false, Mensaje = "Los siguientes DNIs no están registrados" + string.Join(",", faltantes), tipoStatusCode = TipoStatusCode.BadRequest }; 
            } 
            */

            //Comenzar datos partida 
            List<PartidaEntities> listaPartidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));
            PartidaEntities nuevaPartidaEntities = MapeoREQaENT(partidaRequest);
            nuevaPartidaEntities.NumeroPartida = listaPartidas.Count + 1;
            nuevaPartidaEntities.FechaInicio = DateTime.Now;
            nuevaPartidaEntities.Estado = 1;
            nuevaPartidaEntities.TurnoActual = nuevaPartidaEntities.ListaDni[0];
            nuevaPartidaEntities.ConfiguracionTurnos = new List<TurnoEntities>();
            for (int i = 0; i < nuevaPartidaEntities.ListaDni.Count(); i++)
            {
                TurnoEntities turno = new TurnoEntities { NumeroTurno = i + 1, DniJugador = nuevaPartidaEntities.ListaDni[i] };
                nuevaPartidaEntities.ConfiguracionTurnos.Add(turno);
            }
            List<TableroEntities> tablero = Archivos.DeserealizarJson<TableroEntities>(Rutas.ObtenerRuta("tablero"));
            nuevaPartidaEntities.Tablero = tablero;

            //Comenzar datos de detalleUsuario
            foreach (int i in partidaRequest.Dnis)
            {
                DetalleUsuarioEntities detalleUsuarios = new DetalleUsuarioEntities
                {
                    ListaJugadoresPartida = partidaRequest.Dnis,
                    NumeroPartida = nuevaPartidaEntities.NumeroPartida,
                    DniUsuario = i, //Cambio Realizado por Facu, como estaba antes = partidaRequest.Dnis[i]
                    PosicionActual = 0,
                    DineroActual = 5000000,
                    ListaMovimientos = new MovimientosEntities(),
                    Estado = 1,
                };
                Archivos.SerializarJson<DetalleUsuarioEntities>(detalleUsuarios, Rutas.ObtenerRuta("detalleUsuario"));
            }

            Archivos.SerializarJson<PartidaEntities>(nuevaPartidaEntities, Rutas.ObtenerRuta("partida"));
            return new Resultado<int> { Mensaje = $"Partida creada exitosamente. Partida N° {nuevaPartidaEntities.NumeroPartida}", Exito = true, tipoStatusCode = TipoStatusCode.Ok, Data = nuevaPartidaEntities.NumeroPartida };

        }//

        public MovimientoResponse TurnoSiguiente(int numeroPartida)
        {
            List<PartidaEntities> listaPartida = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));
            PartidaEntities partida = listaPartida.FirstOrDefault(n => n.NumeroPartida == numeroPartida);
            if (partida is null)
            {
                return new MovimientoResponse { Success = false, Descripcion = $"No se encontró la partida número {numeroPartida}." };
            }
            MovimientoResponse resultado = LanzarDado(partida);
            //Acomodar para siguiente turno
            TurnoEntities turnoActual = partida.ConfiguracionTurnos[0];
            partida.ConfiguracionTurnos.RemoveAt(0);
            partida.ConfiguracionTurnos.Add(turnoActual);
            partida.TurnoActual = partida.ConfiguracionTurnos[0].DniJugador;
            //Actualizar partida 
            Archivos.ActualizarListaJson<PartidaEntities>(listaPartida, Rutas.ObtenerRuta("partida"));
            return new MovimientoResponse
            {
                Success = true,
                CantidadAlquileres = resultado.CantidadAlquileres,
                CantidadMultas = resultado.CantidadMultas,
                CantidadPremios = resultado.CantidadPremios,
                CantidadPropiedades = resultado.CantidadPropiedades,
                CasilleroDestino = resultado.CasilleroDestino,
                DineroActual = resultado.DineroActual,
                NumeroDado = resultado.NumeroDado,
                Descripcion = "Movimiento realizado correctamente.",
                SiguienteTurno = $"El siguiente turno es del jugador con DNI: {partida.TurnoActual}"
            };
        }

        /*public void ComprarPropiedad(int dniJugador, int casilleroActual)
        {
            string rutaDetalleUsuario = Rutas.ObtenerRuta("detalleUsuario");
            string rutaPartida = Rutas.ObtenerRuta("partida");

            List<DetalleUsuarioEntities> listaDetalleUsuariosAmodificar = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutaDetalleUsuario);
            DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuariosAmodificar.FirstOrDefault(x => x.DniUsuario == dniJugador);
            List<PartidaEntities> listaPartida = Archivos.DeserealizarJson<PartidaEntities>(rutaPartida);
            PartidaEntities partidaActual = listaPartida.FirstOrDefault(p => p.NumeroPartida == detalleUsuarioAmodificar.NumeroPartida);

            TableroEntities casillero = partidaActual.Tablero[casilleroActual];
            if (casillero != null)
            {

                if (casillero.PrecioCompra is not 0 && casillero.DniPropietarioActual is 0 && casillero.PrecioCompra < detalleUsuarioAmodificar.DineroActual)
                {
                    MovimientosEntities movimientos = detalleUsuarioAmodificar.ListaMovimientos;

                    casillero.DniPropietarioActual = dniJugador;
                    detalleUsuarioAmodificar.DineroActual -= casillero.PrecioCompra ?? 0; // Conversión explícita de int? a int
                    movimientos.CompraProvincias += 1;

                    if (movimientos.CompraProvincias >= 12)
                    {
                        FinalizarPartida(detalleUsuarioAmodificar.NumeroPartida);

                    }
                    Archivos.ActualizarListaJson(listaDetalleUsuariosAmodificar, rutaDetalleUsuario);
                    Archivos.ActualizarListaJson(listaPartida, rutaPartida);
                }
            }
        }*/

        public Resultado<int> CambiarEstadoPartida(int numeroPartida, EstadoActualPartida estadoNuevo)
        {
            List<PartidaEntities> partidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));

            if (partidas.Any(p => p.NumeroPartida == numeroPartida))
            {
                PartidaEntities partidaEncontrada = partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida);
                if (partidaEncontrada.Estado == (int)EstadoActualPartida.Finalizada || partidaEncontrada.Estado == (int)EstadoActualPartida.Suspendida)
                {
                    return new Resultado<int> { Exito = false, Mensaje = "No se puede cambiar el estado de una partida finalizada o suspendida", tipoStatusCode = TipoStatusCode.BadRequest };
                }
                partidaEncontrada.Estado = (int)estadoNuevo;
                Archivos.ActualizarListaJson<PartidaEntities>(partidas, Rutas.ObtenerRuta("partida"));
                return new Resultado<int> { Exito = true, Mensaje = "Estado modificado exitosamente", tipoStatusCode = TipoStatusCode.Ok };
            }
            return new Resultado<int> { Exito = false, Mensaje = "No se econtro una partida con ese número", tipoStatusCode = TipoStatusCode.NotFound };
        } //sumar en estado pausado //

        public Resultado<int> FinalizarPartida(int numeroPartida)
        {
            List<PartidaEntities> listaPartidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));
            PartidaEntities partidas = listaPartidas.FirstOrDefault(n => n.NumeroPartida == numeroPartida);
            if (partidas.Estado == (int)EstadoActualPartida.Finalizada || partidas.Estado == (int)EstadoActualPartida.Suspendida)
            {
                return new Resultado<int> { Exito = false, Mensaje = "La partida ya se encuentra finalizada", tipoStatusCode = TipoStatusCode.BadRequest };
            }
            if (partidas != null)
            {
                //Actualizar datos partida
                partidas.Estado = (int)EstadoActualPartida.Finalizada;
                partidas.FechaFin = DateTime.Now;
                partidas.Duracion = partidas.FechaFin - partidas.FechaInicio;
                //Buscar ganador
                Resultado<int> ganador = BuscarGanador(partidas);
                partidas.DniGanador = ganador.Data;
                partidas.MotivoVictoria = ganador.Mensaje;
                Archivos.ActualizarListaJson<PartidaEntities>(listaPartidas, Rutas.ObtenerRuta("partida"));

                //Actualizar estadisticas usuarios 
                List<UsuarioEntities> jugadores = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios"));
                List<UsuarioEntities> jugadoresPartida = jugadores.Where(u => partidas.ListaDni.Contains(u.Dni)).ToList();
                foreach (UsuarioEntities j in jugadoresPartida)
                {
                    j.EstadisticasJugador.PartidasJugadas += 1;

                    if (j.Dni == ganador.Data)
                    {
                        j.EstadisticasJugador.PartidasGanadas += 1;
                    }
                    else
                    {
                        j.EstadisticasJugador.PartidasPerdidas += 1;
                    }
                }
                Archivos.ActualizarListaJson<UsuarioEntities>(jugadores, Rutas.ObtenerRuta("usuarios"));
                return ganador;
            }
            return new Resultado<int> { Exito = false, Mensaje = "No se ha encontrado la partida", tipoStatusCode = TipoStatusCode.NotFound };
        }  //

        public Resultado<List<DetalleUsuarioEntities>> ObtenerDetallesUsuarios(int numeroPartida)
        {
            List<DetalleUsuarioEntities> detalleUsuarios = Archivos.DeserealizarJson<DetalleUsuarioEntities>(Rutas.ObtenerRuta("detalleUsuario")).Where(n => n.NumeroPartida == numeroPartida).ToList();
            if (detalleUsuarios != null)
            {
                return new Resultado<List<DetalleUsuarioEntities>> { Exito = true, Mensaje = "Detalles de usuarios obtenidos correctamente", tipoStatusCode = TipoStatusCode.Ok, Data = detalleUsuarios };
            }
            return new Resultado<List<DetalleUsuarioEntities>> { Exito = false, Mensaje = "No se ha encontrado detalles de usuarios para la partida indicada", tipoStatusCode = TipoStatusCode.NotFound, Data = null };
        }

        private Resultado<int> VerificarUsuarios(List<int> dnisUsuarios)
        {
            List<int> usuariosRegistrados = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios"))
                .Where(f => f.FechaEliminacion == null)
                .Select(u => u.Dni)
                .ToList();

            List<int> usuariosNoRegistrados = dnisUsuarios.Except(usuariosRegistrados).ToList();
            if (usuariosNoRegistrados.Count() > 0)
            {
                return new Resultado<int> { Exito = false, Mensaje = "Los siguientes DNIs no están registrados:" + string.Join(",", usuariosNoRegistrados), tipoStatusCode = TipoStatusCode.BadRequest };
            }
            return new Resultado<int> { Exito = true };
        }

        private Resultado<int> BuscarGanador(PartidaEntities partidaTerminada)
        {
            List<DetalleUsuarioEntities> jugadores = Archivos.DeserealizarJson<DetalleUsuarioEntities>(Rutas.ObtenerRuta("detalleUsuario")).Where(p => p.NumeroPartida == partidaTerminada.NumeroPartida).ToList();
            int dniUsuarioGanador = 0;
            //Verificar condicion unico saldo positivo
            if (jugadores.Where(s => s.DineroActual > 0).Count() == 1)
            {
                dniUsuarioGanador = jugadores.FirstOrDefault(s => s.DineroActual > 0).DniUsuario;
                return new Resultado<int> { Exito = true, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por ser el único con saldo positivo", Data = dniUsuarioGanador, tipoStatusCode = TipoStatusCode.Ok };
            }
            int saldoMayor = 0;
            foreach (DetalleUsuarioEntities i in jugadores)
            {
                if (i.ListaMovimientos.CompraProvincias >= 12)
                {
                    dniUsuarioGanador = i.DniUsuario;
                    return new Resultado<int> { Exito = true, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por poseer 12 o más partidas compradas", Data = dniUsuarioGanador, tipoStatusCode = TipoStatusCode.Ok };
                }
                if (i.DineroActual > saldoMayor)
                {
                    saldoMayor = i.DineroActual;
                    dniUsuarioGanador = i.DniUsuario;
                }
            }
            return new Resultado<int> { Exito = false, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por poseer el mayor saldo", Data = dniUsuarioGanador, tipoStatusCode = TipoStatusCode.Ok }; //Utilizar el false para que se pueda diferenciar en "LanzarDado"
        }

        private MovimientoResponse LanzarDado(PartidaEntities partidaActual)
        {
            string rutadetalle = Rutas.ObtenerRuta("detalleUsuario");
            List<DetalleUsuarioEntities> listaDetalleUsuario = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutadetalle);

            DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuario.FirstOrDefault(x => x.DniUsuario == partidaActual.TurnoActual);

            int casilleroDestino = 0;
            int casillerosTotales = 30;

            Random rand = new Random();

            int numeroDado = rand.Next(1, 7);

            casilleroDestino = detalleUsuarioAmodificar.PosicionActual + numeroDado;
            //Cambio agregado por Facu, si el casillero destino es mayor a 30, que vuelva a empezar desde el 0 + los lugares que le falten (Ver como adaptarlo a cualquier tablero)
            if (casilleroDestino > casillerosTotales)
            {
                //int casillerosTotales = 31;
                //casilleroDestino = (casilleroActual + numeroDado) % casillerosTotales;
                casilleroDestino = casilleroDestino - casillerosTotales - 1;
            }
            MovimientoResponse movimiento = MoverJugador(detalleUsuarioAmodificar.DniUsuario, casilleroDestino);
            movimiento.NumeroDado = numeroDado;
            movimiento.CasilleroDestino = casilleroDestino;

            return movimiento;
        }

        private MovimientoResponse MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
        {
            MovimientoResponse movimientoResponse = new MovimientoResponse();

            string rutaDetalleUsuario = Rutas.ObtenerRuta("detalleUsuario");
            string rutaPartida = Rutas.ObtenerRuta("partida");

            List<DetalleUsuarioEntities> listaDetalleUsuario = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutaDetalleUsuario);

            DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuario.FirstOrDefault(x => x.DniUsuario == dniJugador);
            PartidaEntities partidaActual = Archivos.DeserealizarJson<PartidaEntities>(rutaPartida).FirstOrDefault(x => x.NumeroPartida == detalleUsuarioAmodificar.NumeroPartida);

            if (detalleUsuarioAmodificar is not null)
            {
                detalleUsuarioAmodificar.PosicionActual = casilleroDestino;

                MovimientosEntities movimientos = detalleUsuarioAmodificar.ListaMovimientos;

                if (partidaActual.Tablero[casilleroDestino].TipoCasillero.ToLower() == "premio")
                {
                    detalleUsuarioAmodificar.DineroActual += partidaActual.Tablero[casilleroDestino].Monto ?? 0;
                    movimientos.Premios += 1;
                    movimientoResponse.DineroActual = detalleUsuarioAmodificar.DineroActual;
                    movimientoResponse.CantidadPremios = movimientos.Premios;
                    movimientoResponse.Descripcion = $"Caiste en un casillero con premio, se te suma {partidaActual.Tablero[casilleroDestino].Monto}";

                }
                else
                {
                    if (partidaActual.Tablero[casilleroDestino].TipoCasillero.ToLower() == "multa")
                    {
                        detalleUsuarioAmodificar.DineroActual -= partidaActual.Tablero[casilleroDestino].Monto ?? 0;
                        movimientos.PagoMultas += 1;
                        movimientoResponse.DineroActual = detalleUsuarioAmodificar.DineroActual;
                        movimientoResponse.CantidadMultas = movimientos.PagoMultas;
                        movimientoResponse.Descripcion = $"Caiste en un casillero con multa, se te descuenta {partidaActual.Tablero[casilleroDestino].Monto}";
                    }
                    else
                    {
                        if (partidaActual.Tablero[casilleroDestino].DniPropietarioActual is not 0)
                        {

                            DetalleUsuarioEntities detallePropietario = listaDetalleUsuario.FirstOrDefault(p => p.DniUsuario == partidaActual.Tablero[casilleroDestino].DniPropietarioActual);


                            detalleUsuarioAmodificar.DineroActual -= partidaActual.Tablero[casilleroDestino].Monto ?? 0;

                            detallePropietario.DineroActual += partidaActual.Tablero[casilleroDestino].Monto ?? 0;
                            movimientos.PagosAlquiler += 1;
                            movimientoResponse.DineroActual = detalleUsuarioAmodificar.DineroActual;
                            movimientoResponse.CantidadAlquileres = movimientos.PagosAlquiler;
                            movimientoResponse.Descripcion = $"Caiste en un casillero con alquiler, se te descuenta {partidaActual.Tablero[casilleroDestino].Monto}";


                        }
                        else
                        {
                            if (partidaActual.Tablero[casilleroDestino].PrecioCompra is not 0 && partidaActual.Tablero[casilleroDestino].DniPropietarioActual is 0 && partidaActual.Tablero[casilleroDestino].PrecioCompra < detalleUsuarioAmodificar.DineroActual)
                            {
                                detalleUsuarioAmodificar.DineroActual -= partidaActual.Tablero[casilleroDestino].PrecioCompra ?? 0;
                                movimientos.CompraProvincias += 1;
                                movimientoResponse.DineroActual = detalleUsuarioAmodificar.DineroActual;
                                movimientoResponse.CantidadPropiedades = movimientos.CompraProvincias;
                                movimientoResponse.Descripcion = $"Caiste en un casillero de propiedad sin dueno, se te descuenta {partidaActual.Tablero[casilleroDestino].PrecioCompra} y tienes en tu poder la provincia {partidaActual.Tablero[casilleroDestino].NombreProvincia}";
                            }
                        }
                    }
                }


                //Posible perdida de jugador
                if (detalleUsuarioAmodificar.DineroActual < 0)
                {
                    detalleUsuarioAmodificar.Estado = 2;
                    partidaActual.ConfiguracionTurnos.RemoveAll(t => t.DniJugador == detalleUsuarioAmodificar.DniUsuario);
                    Resultado<int> result = BuscarGanador(partidaActual);
                    if (result.Exito)
                    {
                        FinalizarPartida(partidaActual.NumeroPartida);
                    }
                }
                Archivos.ActualizarListaJson(listaDetalleUsuario, rutaDetalleUsuario);
                return movimientoResponse;
            }
            movimientoResponse.Descripcion = "Caiste en un casillero de provincia sin propietario";
            return movimientoResponse;
        }

        private PartidaEntities MapeoREQaENT(PartidaRequest request)
        {
            return new PartidaEntities
            {
                ListaDni = request.Dnis
            };
        }
        public List<PartidaEntities> ObtenerPartida()
        {
            List<PartidaEntities> listaPartidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));
            return listaPartidas;
        }
    }
    /*Cosas a implementar Dia Martes:
     1. Si el saldoActual del usuario en la partida es menor al alquiler que debe pagar, se debe cambiar su estado en la partida (En juego a Derrotado)
     2. Gestión de turnos, realizar un contador para configurar los turnos, dar de baja de la lista de jugadores en la partida a los usuarios derrotados (Ver si usar remove o poner un if(si usuario.Estado == Derrotado)))
     3. Tema turnos, ver si usar el metodo BuscarGanador constantemente o buscar otra soluciones*/
}
