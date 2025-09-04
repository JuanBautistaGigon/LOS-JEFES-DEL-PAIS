namespace EstancieroEntities
{
    public class UsuarioEntities
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        // estadisticas:
        public int PartidasJugadas { get; set; }
        public int PartidasGanadas { get; set; }
        public int PartidasPerdidas { get; set; }
        public int PartidasPendientes { get; set; }
    }
}
