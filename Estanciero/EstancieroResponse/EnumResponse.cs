using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroResponse
{
    public enum EstadoActualPartida
    {
        EnJuego = 1,
        Finalizada,
        Suspendida,
        Pausada,
    }
    public enum TipoOperacion
    {
        Compra,
        Multa,
        Premio,
    }
    public enum EstadoJudaor
    {
        EnJuego,
        Derrotado,
    }
}
