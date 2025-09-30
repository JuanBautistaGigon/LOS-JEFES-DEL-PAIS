using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class Resultado<T>
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public T? Data { get; set; }
        public TipoStatusCode tipoStatusCode { get; set; }
    }
}
