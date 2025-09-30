using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstancieroData;
using EstancieroEntities;
using EstancieroResponse;
using EstancieroRequest;


namespace EstancieroService
{
    public class PartidaService
    {
        
        public void CrearPartida(PartidaRequest partidaRequest)
        {
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
            nuevaPartidaEntities.Tablero = new List<TableroEntities>(); //duda

            Archivos.SerializarJson<PartidaEntities>(nuevaPartidaEntities, Rutas.ObtenerRuta("partida"));
 
        }
        

        public void CambiarEstadoPartida()
        {
            // Lógica para cambiar el estado de una partida
            // Parametros: IdPartida, NuevoEstado
        }

        public void AgregarJugadorPartida()
        {
            // Lógica para agregar un jugador a una partida
            // Parametros: IdPartida, DniJugador
        }

        // Analizar si es necesario hacer metodos para iniciar y finalizar partida

        private PartidaEntities MapeoREQaENT(PartidaRequest request)
        {
            return new PartidaEntities
            {
                ListaDni = request.Dnis
            };
        }
    }
}
