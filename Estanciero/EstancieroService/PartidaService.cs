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
            
            List<UsuarioEntities> listaUsuariosRegistrados = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios")); 
            List<int> faltantes = partidaRequest.Dnis.Except(listaUsuariosRegistrados.Select(u => u.Dni)).ToList();
            if (faltantes.Any())                
            {
                return new Resultado<int> { Exito = false, Mensaje = "Los siguientes DNIs no están registrados" + string.Join(",", faltantes), tipoStatusCode = TipoStatusCode.BadRequest }; 
            }

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
            

            Archivos.SerializarJson<PartidaEntities>(nuevaPartidaEntities, Rutas.ObtenerRuta("partida"));
            return new Resultado<int> { Mensaje = "Partida creada exitosamente", Exito = true, tipoStatusCode = TipoStatusCode.Ok };
 
        }
        
        public int LanzarDado(int casilleroActual)
        {
            int casilleroDestino = 0;

            Random rand = new Random();

            casilleroDestino = casilleroActual + rand.Next(1, 7);

            return casilleroDestino;
        }
        public void MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
                                                                       // o debe llegar el numero del dado y sumarlo al actual
        {
            int casilleroNuevo = 0;
            string rutaUsuario = Rutas.ObtenerRuta("usuarios");
            string rutaTablero = Rutas.ObtenerRuta("tablero");
            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
            List<TableroEntities> listaCasilleros = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);
            UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x => x.Dni == dniJugador);


            casilleroNuevo = LanzarDado(usuarioAmodificar.CasilleroActual);

            TableroEntities Casillero = listaCasilleros.FirstOrDefault(x => x.NumeroCasillero == casilleroNuevo);

            if (Casillero.TipoCasillero == "premio")
            {
                usuarioAmodificar.Monto += Casillero.Monto ?? 0;
            }
            else
            {
                if (Casillero.TipoCasillero == "multa")
                {
                    usuarioAmodificar.Monto -= Casillero.Monto ?? 0;
                }
                else
                {
                    if (Casillero.DniPropietarioActual is not null)
                    {
                        UsuarioEntities propietario = listaUsuarios.FirstOrDefault(p => p.Dni == Casillero.DniPropietarioActual);
                        usuarioAmodificar.Monto -= Casillero.Monto ?? 0;
                        propietario.Monto += Casillero.Monto ?? 0;
                    }
                }
            }
            usuarioAmodificar.CasilleroActual = casilleroNuevo;

            Archivos.ActualizarListaJson(listaUsuarios, rutaUsuario);
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
        }

        public void FinalizarPartida(int numeroPartida)
        {
            List<PartidaEntities> partidas = Archivos.DeserealizarJson<PartidaEntities>(Rutas.ObtenerRuta("partida"));

            if (partidas.Any(p => p.NumeroPartida == numeroPartida))
            {
                int indice = partidas.FindIndex(p => p.NumeroPartida == numeroPartida);
                partidas[indice].Estado = (int)EstadoActualPartida.Finalizada;
                partidas[indice].FechaFin = DateTime.Now; 
                partidas[indice].Duracion = partidas[indice].FechaFin - partidas[indice].FechaInicio;
            }
        }

        private PartidaEntities MapeoREQaENT(PartidaRequest request)
        {
            return new PartidaEntities
            {
                ListaDni = request.Dnis
            };
        }
    }
}
