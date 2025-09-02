namespace EstancieroEntities
{
    public class UsuarioEntities
    {
        public int Dni { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public List<EstadisticaEntities> Estadisticas { get; set; }
    }
}
