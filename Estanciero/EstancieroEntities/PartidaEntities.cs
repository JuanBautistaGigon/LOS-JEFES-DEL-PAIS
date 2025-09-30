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
        public int NumeroPartida { get; set; } //
        public DateTime FechaInicio { get; set; } //
        public DateTime? FechaFin { get; set; } //al final 
        public TimeSpan? Duracion { get; set; } //al final 
        public int Estado { get; set; } //
        public int TurnoActual { get; set; } // 
        public List<TurnoEntities> ConfiguracionTurnos { get; set; } //
        public List<TableroEntities> Tablero { get; set; } // 
        public List<int> ListaDni { get; set; } //
        public int? DniGanador { get; set; } //al final 
        public string? MotivoVictoria { get; set; } //al final
    }
}
