using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class Resultado
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; }
        public UsuarioEntities Usuario { get; set; }
    }
}
