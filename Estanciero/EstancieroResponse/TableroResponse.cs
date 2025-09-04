namespace EstancieroResponse
{
    public class TableroResponse
    {
        public int NumeroCasillero { get; set; }
        public string TipoCasillero { get; set; }
        public string NombreProvincia { get; set; }
        public int PrecioCompra { get; set; }
        public int PrecioAlquiler { get; set; }
        public int? Monto { get; set; }
        public int DniPropietarioActual { get; set; }
    }
}
