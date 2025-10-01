using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class DetalleUsuarioEntities
    {
        public List<int> ListaJugadoresPartida { get; set; } //sacarlo? 
        public int NumeroPartida { get; set; }
        public int DniUsuario { get; set; }
        public int PosicionActual { get; set; }
        public int DineroActual { get; set; }
        public MovimientosEntities ListaMovimientos { get; set; }
        public int Estado { get; set; } // En juego = 1 o Derrotado = 2
    }
}
