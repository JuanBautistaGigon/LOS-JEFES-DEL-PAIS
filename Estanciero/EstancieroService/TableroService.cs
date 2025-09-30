using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EstancieroData;
using EstancieroEntities;

namespace EstancieroService
{
    public class TableroService
    {
<<<<<<< HEAD
//<<<<<<< HEAD
=======
        //<<<<<<< HEAD
>>>>>>> origin/main
        //public void MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
        //                                                              // o debe llegar el numero del dado y sumarlo al actual
        //{
        //    int casilleroNuevo = 0;
        //    string rutaUsuario = Rutas.ObtenerRutaCompletaTablero(); 
        //    List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
        //    UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x=> x.Dni == dniJugador);

        //    usuarioAmodificar.CasilleroActual = LanzarDado(usuarioAmodificar.CasilleroActual);



        //}
<<<<<<< HEAD
//=======
=======
        //=======
>>>>>>> origin/main

        public void MoverJugador(int dniJugador, int casilleroDestino) // preguntar si puede llegar como parametro el casillero,
                                                                       // o debe llegar el numero del dado y sumarlo al actual
        {
            int casilleroNuevo = 0;
            string rutaUsuario = Rutas.ObtenerRuta("usuarios");
            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
            UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x => x.Dni == dniJugador);

            usuarioAmodificar.CasilleroActual = LanzarDado(usuarioAmodificar.CasilleroActual);

            Archivos.ActualizarListaJson(listaUsuarios, "usuarios");
        }
<<<<<<< HEAD
//>>>>>>> origin/main
=======
        //>>>>>>> origin/main
>>>>>>> origin/main
        public int LanzarDado(int casilleroActual)
        {
            int casilleroDestino = 0;

            Random rand = new Random();

            casilleroDestino = casilleroActual + rand.Next(1, 7);

            return casilleroDestino;
        }
        public void ComprarPropiedad(int dniJugador, int casilleroActual)
        {
            string rutaUsuario = Rutas.ObtenerRuta("usuarios");
            string rutaTablero = Rutas.ObtenerRuta("tablero");

            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
            List<TableroEntities> listaTablero = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);

            TableroEntities casillero = listaTablero.FirstOrDefault(x => x.NumeroCasillero == casilleroActual);
            if (casillero != null)
            {
                UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x => x.Dni == dniJugador);
                if (casillero.PrecioCompra is not 0 && casillero.DniPropietarioActual is 0 && casillero.PrecioCompra < usuarioAmodificar.Monto)
                {
                    casillero.DniPropietarioActual = dniJugador;
                    usuarioAmodificar.Monto -= casillero.PrecioCompra;

                    Archivos.ActualizarListaJson(listaUsuarios, "usuarios");
                    Archivos.ActualizarListaJson(listaTablero, "tablero");
                }
            }
            // Lógica para que un jugador compre una propiedad
            // Parametros: DniJugador, Casillero    
        }
        public void CobrarAlquiler(int dniJugador, int casilleroActual)
        {

            string rutaUsuario = Rutas.ObtenerRuta("usuarios");
            string rutaTablero = Rutas.ObtenerRuta("tablero");

            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(rutaUsuario);
            List<TableroEntities> listaTablero = Archivos.DeserealizarJson<TableroEntities>(rutaTablero);

            TableroEntities casillero = listaTablero.FirstOrDefault(x => x.NumeroCasillero == casilleroActual);
            if (casillero != null)
            {
                UsuarioEntities usuarioAmodificar = listaUsuarios.FirstOrDefault(x => x.Dni == dniJugador);
                if (casillero.PrecioAlquiler is not 0 && casillero.DniPropietarioActual is not 0 && casillero.PrecioAlquiler < usuarioAmodificar.Monto) // preguntar que pasa si el monto es menor al precio de alquiler
                {
                    usuarioAmodificar.Monto -= casillero.PrecioAlquiler;

                    Archivos.ActualizarListaJson(listaUsuarios, "usuarios");
                }
            }

            // Lógica para cobrar alquiler a un jugador
            // Parametros: DniPropietario, DniInquilino, Casillero
        }
    }
}
