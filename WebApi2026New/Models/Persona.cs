using System.ComponentModel.DataAnnotations;

namespace WebApi2026New.Models
{
    public class Persona
    {
        [Key]
        public int idPersona { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int edad { get; set; }
        public string correo { get; set; }
        public string password { get; set; }

        public ICollection<Direccion> Direcciones { get; set; } = new List<Direccion>();
    }
}
