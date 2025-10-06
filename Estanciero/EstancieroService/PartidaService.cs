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
            return new Resultado<int> { Mensaje = "Partida creada exitosamente", Exito = true, tipoStatusCode = TipoStatusCode.Ok };

        }//

        public MovimientoResponse LanzarDado(int dniJugador, int casilleroActual)
        {

            int casilleroDestino = 0;

            Random rand = new Random();

            int numeroDado = rand.Next(1, 7);

            casilleroDestino = casilleroActual + numeroDado;
            //Cambio agregado por Facu, si el casillero destino es mayor a 30, que vuelva a empezar desde el 0 + los lugares que le falten (Ver como adaptarlo a cualquier tablero)
            if (casilleroDestino > 30)
            {
                int casillerosTotales = 31;
                casilleroDestino = (casilleroActual + numeroDado) % casillerosTotales;
            }
            MovimientoResponse movimiento = MoverJugador(dniJugador, casilleroDestino);
            movimiento.NumeroDado = numeroDado;
            movimiento.CasilleroDestino = casilleroDestino;

            return movimiento;
        }

        public MovimientoResponse MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
        {
            MovimientoResponse movimientoResponse = new MovimientoResponse();

            string rutaTablero = Rutas.ObtenerRuta("tablero");
            string rutaDetalleUsuario = Rutas.ObtenerRuta("detalleUsuario");
            string rutaPartida = Rutas.ObtenerRuta("partida");

            List<TableroEntities> listaCasilleros = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);

            List<DetalleUsuarioEntities> listaDetalleUsuario = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutaDetalleUsuario);

            DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuario.FirstOrDefault(x => x.DniUsuario == dniJugador);
            PartidaEntities partidaActual = Archivos.DeserealizarJson<PartidaEntities>(rutaPartida).FirstOrDefault(x => x.NumeroPartida == detalleUsuarioAmodificar.NumeroPartida);

            if (detalleUsuarioAmodificar is not null)
            {
                detalleUsuarioAmodificar.PosicionActual = casilleroDestino;

                TableroEntities casillero = listaCasilleros.FirstOrDefault(x => x.NumeroCasillero == casilleroDestino);
                MovimientosEntities movimientos = detalleUsuarioAmodificar.ListaMovimientos;

                if (casillero.TipoCasillero.ToLower() == "premio")
                {
                    detalleUsuarioAmodificar.DineroActual += casillero.Monto ?? 0;
                    movimientos.Premios += 1;
                    movimientoResponse.CantidadPremios = movimientos.Premios;
                    movimientoResponse.Descripcion = $"Caiste en un casillero con premio, se te suma {casillero.Monto}";

                }
                else
                {
                    if (casillero.TipoCasillero.ToLower() == "multa")
                    {
                        detalleUsuarioAmodificar.DineroActual -= casillero.Monto ?? 0;
                        movimientos.PagoMultas += 1;
                        movimientoResponse.CantidadMultas = movimientos.PagoMultas;
                        movimientoResponse.Descripcion = $"Caiste en un casillero con multa, se te descuenta {casillero.Monto}";
                    }
                    else
                    {
                        if (casillero.DniPropietarioActual is not null)
                        {

                            DetalleUsuarioEntities detallePropietario = listaDetalleUsuario.FirstOrDefault(p => p.DniUsuario == casillero.DniPropietarioActual);


                            detalleUsuarioAmodificar.DineroActual -= casillero.Monto ?? 0;

                            detallePropietario.DineroActual += casillero.Monto ?? 0;
                            movimientos.PagosAlquiler += 1;
                            movimientoResponse.CantidadAlquileres = movimientos.PagosAlquiler;
                            movimientoResponse.Descripcion = $"Caiste en un casillero con alquiler, se te descuenta {casillero.Monto}";


                        }
                    }
                }
                Archivos.ActualizarListaJson(listaDetalleUsuario, rutaDetalleUsuario);
                Archivos.ActualizarListaJson(listaCasilleros, rutaTablero);

                if (detalleUsuarioAmodificar.DineroActual < 0)
                {
                    Resultado<int> result = BuscarGanador(partidaActual);
                    if (result.Exito)
                    {
                        FinalizarPartida(partidaActual.NumeroPartida);
                    }
                    else
                    {
                        detalleUsuarioAmodificar.Estado = 2;
                    }
                }


                return movimientoResponse;
            }
            movimientoResponse.Descripcion = "Caiste en un casillero de provincia sin propietario";
            return movimientoResponse;
        }

        public void ComprarPropiedad(int dniJugador, int casilleroActual)
        {
            string rutaDetalleUsuario = Rutas.ObtenerRuta("detalleUsuario");
            string rutaTablero = Rutas.ObtenerRuta("tablero");
            string rutaPartida = Rutas.ObtenerRuta("partida");

            List<DetalleUsuarioEntities> listaDetalleUsuariosAmodificar = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutaDetalleUsuario);
            List<TableroEntities> listaTablero = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);

            TableroEntities casillero = listaTablero.FirstOrDefault(x => x.NumeroCasillero == casilleroActual);
            if (casillero != null)
            {
                DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuariosAmodificar.FirstOrDefault(x => x.DniUsuario == dniJugador);
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
                    Archivos.ActualizarListaJson(listaTablero, rutaTablero);
                }
            }
            // Lógica para que un jugador compre una propiedad
            // Parametros: DniJugador, Casillero    
        }

        public Resultado<int> CambiarEstadoPartida(int numeroPartida, EstadoActualPartida estadoNuevo)
        {
            List<PartidaEntities> partidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));

            if (partidas.Any(p => p.NumeroPartida == numeroPartida))
            {
                partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida).Estado = (int)estadoNuevo;
                Archivos.ActualizarListaJson<PartidaEntities>(partidas, Rutas.ObtenerRuta("partida"));
                return new Resultado<int> { Exito = true, Mensaje = "Estado modificado exitosamente" };
            }
            return new Resultado<int> { Exito = false, Mensaje = "No se econtro una partida con ese número" };
        } //sumar en estado pausado //

        public Resultado<int> FinalizarPartida(int numeroPartida)
        {
            List<PartidaEntities> listaPartidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));
            PartidaEntities partidas = listaPartidas.FirstOrDefault(n => n.NumeroPartida == numeroPartida);
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

        private PartidaEntities MapeoREQaENT(PartidaRequest request)
        {
            return new PartidaEntities
            {
                ListaDni = request.Dnis
            };
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
                return new Resultado<int> { Exito = true, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por ser el único con saldo positivo", Data = dniUsuarioGanador };
            }
            int saldoMayor = 0;
            foreach (DetalleUsuarioEntities i in jugadores)
            {
                if (i.ListaMovimientos.CompraProvincias >= 12)
                {
                    dniUsuarioGanador = i.DniUsuario;
                    return new Resultado<int> { Exito = true, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por poseer 12 o más partidas compradas", Data = dniUsuarioGanador };
                }
                if (i.DineroActual > saldoMayor)
                {
                    saldoMayor = i.DineroActual;
                    dniUsuarioGanador = i.DniUsuario;
                }
            }
            return new Resultado<int> { Exito = false, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por poseer el mayor saldo", Data = dniUsuarioGanador };
        }
    }
    /*Cosas a implementar Dia Martes:
     1. Si el saldoActual del usuario en la partida es menor al alquiler que debe pagar, se debe cambiar su estado en la partida (En juego a Derrotado)
     2. Gestión de turnos, realizar un contador para configurar los turnos, dar de baja de la lista de jugadores en la partida a los usuarios derrotados (Ver si usar remove o poner un if(si usuario.Estado == Derrotado)))
     3. Tema turnos, ver si usar el metodo BuscarGanador constantemente o buscar otra soluciones*/
}
