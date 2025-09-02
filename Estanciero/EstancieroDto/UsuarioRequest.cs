using System.ComponentModel.DataAnnotations;

namespace EstancieroDto
{
    public class UsuarioRequest
    {
        [Required(ErrorMessage = "el dni es obligatorio")]
        [Range(10000000, 100000000, ErrorMessage = "el dni debe tener 8 digitos")]
        public int Dni { get; set; }


    }
}
