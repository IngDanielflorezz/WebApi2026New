using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi2026New.Models
{
    public class Direccion
    {
        [Key]
        public int idDireccion { get; set; }
        public string? calle { get; set; }
        public string? ciudad { get; set; }
        public string? departamento { get; set; }
        public string? pais { get; set; }
        public string? codigoPostal { get; set; }
        [ForeignKey("Persona")]
        public int PersonaId { get; set; }
        public Persona? persona { get; set; }
    }
}
