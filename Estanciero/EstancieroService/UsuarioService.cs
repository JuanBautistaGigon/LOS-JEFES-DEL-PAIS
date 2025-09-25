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
            UsuarioEntities usuarioEntities = new UsuarioEntities();
            usuarioEntities.Dni = usuario.Dni;
            usuarioEntities.Nombre = usuario.Nombre;
            usuarioEntities.Email = usuario.Email;
            usuarioEntities.Monto = 5000000;
            usuarioEntities.FechaRegistro = DateTime.Now;
            usuarioEntities.FechaEliminacion = null;
            usuarioEntities.EstadisticasJugador = new EstadisticasJugador();
            string ruta = Rutas.ObtenerRutaCompletaUsuarios();
            Archivos.SerializarJson(usuarioEntities, ruta);
            return new Resultado { Exito = true, Mensaje = "Usuario creado con exito", Usuario = usuarioEntities };
        }
        public void ModificarDinero(int dni, int monto)
        {
            // Lógica para modificar el dinero de un usuario
            //Parametros: Dni, Cantidad
            //Posible Motivo: Comprar, Multas o Premios
        }
        public void TransferirDinero()
        {
            // Lógica para transferir dinero entre usuarios
            //Parametros: DniEmisor, DniReceptor, Cantidad
            //Posibles Motivos: Cobrar o Pagar
        }

    }
}
