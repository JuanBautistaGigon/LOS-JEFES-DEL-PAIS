using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroService
{
    public class TableroService
    {
        public void MoverJugador()
        {
            // Lógica para mover un jugador en el tablero
            // Parametros: DniJugador, CasilleroDestino
        }
        public void LanzarDado()
        {
            // Lógica para lanzar el dado y obtener un número aleatorio
            // Parametros: DniJugador
            // Retorna: CasilleroDestino, es decir, sumar posicion Actual + NumeroDado
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
