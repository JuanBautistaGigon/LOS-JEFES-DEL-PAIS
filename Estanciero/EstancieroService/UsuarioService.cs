using EstancieroData;
using EstancieroEntities;
using EstancieroRequest;
using EstancieroResponse;

namespace EstancieroService
{
    public class UsuarioService
    {
        public Resultado<UsuarioEntities> CrearUsuario(UsuarioRequest usuario)
        {
            //Lógica para crear un nuevo usuario
            //Parametros: usuarioRequest, revisar si es necesario mandar un usuario entero o solo DNI
            var usuarioEntities = MapeoRequestAEntities(usuario);
            string ruta = Rutas.ObtenerRuta("usuarios");
            Archivos.SerializarJson(usuarioEntities, ruta);
            return new Resultado<UsuarioEntities> { Exito = true, Mensaje = "Usuario creado con exito", Data = usuarioEntities, tipoStatusCode = TipoStatusCode.Ok };
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
