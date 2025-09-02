using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class TableroEntities
    {
        public int NumeroCasillero { get; set; }
        public string TipoCasillero { get; set; }
        public string NombreProvincia { get; set; }
        public int PrecioCompra { get; set; }
        public int PrecioAlquiler { get; set; }
        public int? Monto { get; set; }
        public int DniPropietarioActual { get; set; }
    }
}
