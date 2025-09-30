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
        /*
        public PartidaResponse CrearPartida(PartidaRequest partidaRequest)
        {

            /*string ruta = Rutas.ObtenerRutaCompletaUsuarios();
            List<UsuarioEntities> listadnis = Archivos.DeserealizarJson<UsuarioEntities>(ruta);

            PartidaEntities partida = listadnis.FirstOrDefault(x=> x.Dni.Any);



            
        };
        */

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
