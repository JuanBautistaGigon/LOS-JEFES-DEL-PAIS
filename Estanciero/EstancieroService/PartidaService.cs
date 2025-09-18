using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstancieroData;
using EstancieroEntities;
using EstancieroResponse;


namespace EstancieroService
{
    public class PartidaService
    {
        public PartidaResponse CrearPartida(List<int> listadni)
        {
            string ruta = Rutas.ObtenerRutaCompletaPartidas();
            List<PartidaResponse> response = Archivos.DeserealizarJson<PartidaResponse>(ruta);
            
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
    }
}
