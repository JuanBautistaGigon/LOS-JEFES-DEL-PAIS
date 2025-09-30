using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EstancieroRequest
{
    public class PartidaRequest
    {
        [MinLength (2)]
        [MaxLength (4)]
        [Range(10000000,99999999)]
        public List<int> Dnis {  get; set; }
    }
}
