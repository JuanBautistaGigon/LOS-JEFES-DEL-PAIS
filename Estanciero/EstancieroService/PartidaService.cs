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
            foreach (int i in partidaRequest.Dnis) {
                DetalleUsuarioEntities detalleUsuarios = new DetalleUsuarioEntities
                {
                    ListaJugadoresPartida = partidaRequest.Dnis,
                    NumeroPartida = nuevaPartidaEntities.NumeroPartida,
                    DniUsuario = partidaRequest.Dnis[i], 
                    PosicionActual = 0, 
                    DineroActual = 5000000, 
                    ListaMovimientos = new MovimientosEntities(), 
                    Estado = 1, 
                };
                Archivos.SerializarJson<DetalleUsuarioEntities>(detalleUsuarios, Rutas.ObtenerRuta("detalleUsuario")); 
            }

            Archivos.SerializarJson<PartidaEntities>(nuevaPartidaEntities, Rutas.ObtenerRuta("partida"));
            return new Resultado<int> { Mensaje = "Partida creada exitosamente", Exito = true, tipoStatusCode = TipoStatusCode.Ok };
 
        }

        public int LanzarDado(int dniJugador, int casilleroActual)
        {
            int casilleroDestino = 0;

            Random rand = new Random();

            int numeroDado = rand.Next(1, 7);

            casilleroDestino = casilleroActual + numeroDado;

            MoverJugador(dniJugador, casilleroDestino);

            return numeroDado;
        }


        public DetalleUsuarioEntities MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
        {
            string rutaTablero = Rutas.ObtenerRuta("tablero");
            string rutaDetalleUsuario = Rutas.ObtenerRuta("detalleUsuario");
            List<TableroEntities> listaCasilleros = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);

            List<DetalleUsuarioEntities> listaDetalleUsuario = Archivos.DeserealizarJson<DetalleUsuarioEntities>(rutaDetalleUsuario);

            DetalleUsuarioEntities detalleUsuarioAmodificar = listaDetalleUsuario.FirstOrDefault(x => x.DniUsuario == dniJugador);

            if (detalleUsuarioAmodificar is not null)
            {
                detalleUsuarioAmodificar.PosicionActual = casilleroDestino;

                TableroEntities Casillero = listaCasilleros.FirstOrDefault(x => x.NumeroCasillero == casilleroDestino);
                MovimientosEntities movimientos = detalleUsuarioAmodificar.ListaMovimientos;

                if (Casillero.TipoCasillero == "premio")
                {
                    detalleUsuarioAmodificar.DineroActual += Casillero.Monto ?? 0;
                    movimientos.Premios += 1;
                }
                else
                {
                    if (Casillero.TipoCasillero == "multa")
                    {
                        detalleUsuarioAmodificar.DineroActual -= Casillero.Monto ?? 0;
                        movimientos.PagoMultas += 1;
                    }
                    else
                    {
                        if (Casillero.DniPropietarioActual is not null)
                        {
                            DetalleUsuarioEntities detallePropietario = listaDetalleUsuario.FirstOrDefault(p => p.DniUsuario == Casillero.DniPropietarioActual);
                            detalleUsuarioAmodificar.DineroActual -= Casillero.Monto ?? 0;
                            detallePropietario.DineroActual += Casillero.Monto ?? 0;
                            movimientos.PagosAlquiler += 1;
                        }
                    }
                }
                Archivos.ActualizarListaJson(listaDetalleUsuario, rutaDetalleUsuario);
                Archivos.ActualizarListaJson(listaCasilleros, rutaTablero);

                return detalleUsuarioAmodificar;
            }

            return null;
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
                        PartidaEntities partida = Archivos.DeserealizarJson<PartidaEntities>(rutaPartida).FirstOrDefault(x => x.NumeroPartida == detalleUsuarioAmodificar.NumeroPartida);
                        //BuscarGanador(partida);
                        ;
                    }


                    Archivos.ActualizarListaJson(listaDetalleUsuariosAmodificar, rutaDetalleUsuario);
                    Archivos.ActualizarListaJson(listaTablero, rutaTablero);
                }
            }
            // Lógica para que un jugador compre una propiedad
            // Parametros: DniJugador, Casillero    
        }

        public Resultado<PartidaEntities> CambiarEstadoPartida(int numeroPartida, EstadoActualPartida estadoNuevo)
        {
            List<PartidaEntities> partidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));

            if (partidas.Any(p => p.NumeroPartida == numeroPartida))
            {
                partidas.FirstOrDefault(p => p.NumeroPartida == numeroPartida).Estado = (int)estadoNuevo;
                Archivos.ActualizarListaJson<PartidaEntities>(partidas, Rutas.ObtenerRuta("partida"));
                return new Resultado<PartidaEntities> { Exito = true, Mensaje = "Estado modificado exitosamente" };
            }
            return new Resultado<PartidaEntities> { Exito = false, Mensaje = "No se econtro una partida con ese número" };
        } //sumar en estado pausado

        public void FinalizarPartida(int numeroPartida, EstadoActualPartida estado)
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
                foreach (UsuarioEntities j in  jugadoresPartida)
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
            }
        } //devolver algo? 

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
                if (i.DineroActual >  saldoMayor)
                {
                    saldoMayor = i.DineroActual;
                    dniUsuarioGanador = i.DniUsuario;
                }
            }
            return new Resultado<int> { Exito = false, Mensaje = $"El jugador {dniUsuarioGanador} ha ganado la partida por poseer el mayor saldo", Data = dniUsuarioGanador };
        }
    }
}
