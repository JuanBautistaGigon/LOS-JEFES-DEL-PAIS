using System.Diagnostics;
using Newtonsoft.Json;

namespace EstancieroData
{
    
    public static class Archivos
    {
        //public static string parteDeLaRuta = "../EstancieroData/DataBase";
        //public static string rutaArchivo = Path.Combine(parteDeLaRuta, "estanciero.json");
        //public static string rutaCompleta = Path.GetFullPath(rutaArchivo); 

        public static void SerializarJson<T>(T objeto, string ruta)
        {
            string directory = Path.GetDirectoryName(ruta); //
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            List<T> lista = DeserealizarJson<T>(ruta); 
            lista.Add(objeto);

            string json = JsonConvert.SerializeObject(lista, Formatting.Indented);
            File.WriteAllText(ruta, json); //
        }

        public static List<T> DeserealizarJson<T>(string ruta)
        {
            if (!File.Exists(ruta)) //
            {
                return new List<T>(); 
            } else
            {
                string json = File.ReadAllText(ruta); // 
                List<T> lista = JsonConvert.DeserializeObject<List<T>>(json);
                return lista;
            }
        }

        public static void ActualizarListaJson<T>(List<T> lista, string ruta)
        {
            string directory = Path.GetDirectoryName(ruta);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string json = JsonConvert.SerializeObject(lista, Formatting.Indented);
            File.WriteAllText(ruta, json); 
        }

    }
}
