using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroEntities
{
    public class PartidaEntities
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public int? Duracion { get; set; }
        public int Estado { get; set; }
        public int TurnoActual { get; set; }
        public List<TurnoEntities> Configuracion { get; set; } //Revisar
        public List<int> ListaDni { get; set; }
        public int? DniGanador { get; set; }
        public string? MotivoVictoria { get; set; }
    }
}
