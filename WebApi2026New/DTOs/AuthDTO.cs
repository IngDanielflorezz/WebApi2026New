
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi2026New.DTOs
{
    public class RegistroDTO
    {
        [Required]
        public string nombre { get; set; } = string.Empty;

        [Required]
        public string apellido { get; set; } = string.Empty;

        public int edad { get; set; }

        [Required, EmailAddress]
        public string correo { get; set; } = string.Empty;

        [Required, MinLength(8)]
        public string password { get; set; } = string.Empty;
    }

    public class LoginDTO
    {
        [Required, EmailAddress]
        public string correo { get; set; } = string.Empty;

        [Required]
        public string password { get; set; } = string.Empty;
    }

    public class LoginRespuestaDTO
    {
        public string token { get; set; } = string.Empty;
        public DateTime expira { get; set; }
        public int idPersona { get; set; }
        public string nombre { get; set; } = string.Empty;
    }
}
