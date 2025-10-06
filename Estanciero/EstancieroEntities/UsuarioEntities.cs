namespace EstancieroEntities
{
    public class UsuarioEntities
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public EstadisticasJugador EstadisticasJugador { get; set; }
    }
}
