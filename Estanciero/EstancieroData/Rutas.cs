using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstancieroData
{
    public static class Rutas
    {
        public static string parteDeLaRuta = "../EstancieroData/DataBase";
        public static string ObtenerRutaCompletaUsuarios()
        {
            string rutaArchivo = Path.Combine(parteDeLaRuta, "usuarios.json");
            string rutaCompleta = Path.GetFullPath(rutaArchivo); 
            return rutaCompleta;
        }
        public static string ObtenerRutaCompletaTablero()
        {
            string rutaArchivo = Path.Combine(parteDeLaRuta, "tablero.json");
            string rutaCompleta = Path.GetFullPath(rutaArchivo);
            return rutaCompleta;
        }
        public static string ObtenerRutaCompletaPartidas()
        {
            string rutaArchivo = Path.Combine(parteDeLaRuta, "partidas.json");
            string rutaCompleta = Path.GetFullPath(rutaArchivo);
            return rutaCompleta;
        }
    }
}
