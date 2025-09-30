using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstancieroData;
using EstancieroEntities;

namespace EstancieroService
{
    public class TableroService
    {
        public void MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
                                                                      // o debe llegar el numero del dado y sumarlo al actual
        {
            int casilleroNuevo = 0;
            string rutaUsuario = Rutas.ObtenerRutaCompletaTablero(); 
            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
            UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x=> x.Dni == dniJugador);

            usuarioAmodificar.CasilleroActual = LanzarDado(usuarioAmodificar.CasilleroActual);
   
            
            
        }
        public int LanzarDado(int casilleroActual)
        {
            int casilleroDestino = 0;


            Random rand = new Random();

            casilleroDestino = casilleroActual + rand.Next(1,7);

            return casilleroDestino;
        }
        public void ComprarPropiedad()
        {
            // Lógica para que un jugador compre una propiedad
            // Parametros: DniJugador, Casillero    
        }
        public void CobrarAlquiler()
        {
            // Lógica para cobrar alquiler a un jugador
            // Parametros: DniPropietario, DniInquilino, Casillero
        }
    }
}
