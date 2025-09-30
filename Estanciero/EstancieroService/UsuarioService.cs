using EstancieroData;
using EstancieroEntities;
using EstancieroRequest;
using EstancieroResponse;

namespace EstancieroService
{
    public class UsuarioService
    {
        public Resultado CrearUsuario(UsuarioRequest usuario)
        {
            //Lógica para crear un nuevo usuario
            //Parametros: usuarioRequest, revisar si es necesario mandar un usuario entero o solo DNI
            var usuarioEntities = MapeoRequestAEntities(usuario);
            string ruta = Rutas.ObtenerRuta("usuarios");
            Archivos.SerializarJson(usuarioEntities, ruta);
            return new Resultado { Exito = true, Mensaje = "Usuario creado con exito", Usuario = usuarioEntities };
        }
        public Resultado ModificarDinero(int dni, int monto, TipoOperacion tipoOperacion)
        {
            // Lógica para modificar el dinero de un usuario
            //Parametros: Dni, Cantidad, TipoOperacion
            //Posible Motivo: Comprar, Multas o Premios
            var listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios"));
            var usuario = listaUsuarios.FirstOrDefault(u => u.Dni == dni);
            if (usuario == null)
            {
                return new Resultado { Exito = false, Mensaje = "Usuario no encontrado" };
            }
            else
            {
                if (tipoOperacion == TipoOperacion.Premio)
                {
                    usuario.Monto += monto;
                    //Cambiar por actualizar
                    Archivos.ActualizarListaJson(listaUsuarios, Rutas.ObtenerRuta("usuarios"));
                    return new Resultado { Exito = true, Mensaje = "Dinero agregado correctamente" };
                }
                else
                {
                    usuario.Monto -= monto;
                    //Cambiar por actualizar 
                    Archivos.ActualizarListaJson(listaUsuarios, Rutas.ObtenerRuta("usuarios"));
                    if (tipoOperacion == TipoOperacion.Multa)
                    {
                        return new Resultado { Exito = true, Mensaje = "Multa aplicada correctamente" };
                    }
                    else
                    {
                        return new Resultado { Exito = true, Mensaje = "Compra realizada correctamente" };
                    }
                }
            }
        }
        public Resultado TransferirDinero(int dniEmisor, int dniReceptor, int monto)
        {
            // Lógica para transferir dinero entre usuarios
            //Parametros: DniEmisor, DniReceptor, Cantidad
            //Posibles Motivos: Cobrar o Pagar
            var listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(Rutas.ObtenerRuta("usuarios"));
            var usuarioEmisor = listaUsuarios.FirstOrDefault(u => u.Dni == dniEmisor);
            var usuarioReceptor = listaUsuarios.FirstOrDefault(u => u.Dni == dniReceptor);
            if (usuarioEmisor == null || usuarioReceptor == null)
            {
                return new Resultado { Exito = false, Mensaje = "Uno o ambos usuarios no existen." };
            }
            else
            {
                usuarioEmisor.Monto -= monto;
                usuarioReceptor.Monto += monto;
                Archivos.ActualizarListaJson(listaUsuarios, Rutas.ObtenerRuta("usuarios"));
                return new Resultado { Exito = true, Mensaje = "Transferencia realizada con exito." };
            }
        }
        public List<UsuarioEntities> ObtenerUsuarios()
        {
            string ruta = Rutas.ObtenerRuta("usuarios");
            List<UsuarioEntities> listaUsuarios = Archivos.DeserealizarJson<UsuarioEntities>(ruta);
            return listaUsuarios;
        }
        private UsuarioEntities MapeoRequestAEntities(UsuarioRequest usuario)
        {
            UsuarioEntities usuarioEntities = new UsuarioEntities();
            usuarioEntities.Dni = usuario.Dni;
            usuarioEntities.Nombre = usuario.Nombre;
            usuarioEntities.Email = usuario.Email;
            usuarioEntities.Monto = 5000000;
            usuarioEntities.FechaRegistro = DateTime.Now;
            usuarioEntities.FechaEliminacion = null;
            usuarioEntities.EstadisticasJugador = new EstadisticasJugador();
            return usuarioEntities;
        }

    }
}
