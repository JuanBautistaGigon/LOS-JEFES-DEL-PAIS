using System.ComponentModel.DataAnnotations;

namespace EstancieroRequest
{
    public class UsuarioRequest
    {
        [Required(ErrorMessage = "el dni es obligatorio")]
        [Range(10000000, 100000000, ErrorMessage = "el dni debe tener 8 digitos")]
        public int Dni { get; set; }

        [Required(ErrorMessage = "el nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "el nombre es obligatorio")]
        [EmailAddress(ErrorMessage = "sintaxis invalida de email (xxxx@xxx.xx)")]
        public string Email { get; set; }


    }
}
