using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroResponse
{
    public class MovimientoResponse
    {
        public int NumeroDado { get; set; }
        public int CasilleroDestino { get; set; }
        public int DineroActual { get; set; }
        public int? CantidadPremios { get; set; }
        public string Descripcion { get; set; }
        public int? CantidadMultas { get; set; }
        public int? CantidadAlquileres { get; set; }
        public int? CantidadPropiedades { get; set; }
        public bool Success { get; set; }
        public string SiguienteTurno { get; set; }
        public bool Termino { get; set; }
        public int Ganador {  get; set; }
        public int Perdedor { get; set; }
    }
}
