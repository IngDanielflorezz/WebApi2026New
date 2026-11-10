namespace WebApi2026New.DTOs
{
    public class DireccionDTO
    {
        public int idDireccion { get; set; }
        public string calle { get; set; } = string.Empty;
        public string ciudad { get; set; } = string.Empty;
        public string departamento { get; set; } = string.Empty;
        public string codigoPostal { get; set; } = string.Empty;
        public string pais { get; set; } = string.Empty;
        public int personaId { get; set; }
        public string? nombrePersona { get; set; }
    }

    public class DireccionCreateDTO
    {
        public string? calle { get; set; }
        public string? ciudad { get; set; }
        public string? departamento { get; set; }
        public string? pais { get; set; }
        public string? codigoPostal { get; set; }
        public int PersonaId { get; set; }
    }

    public class DireccionUpdateDTO
    {
        public string? calle { get; set; }
        public string? ciudad { get; set; }
        public string? departamento { get; set; }
        public string? pais { get; set; }
        public string? codigoPostal { get; set; }
    }

    public class DireccionReadDTO
    {
        public int idDireccion { get; set; }
        public string? calle { get; set; }
        public string? ciudad { get; set; }
        public string? departamento { get; set; }
        public string? pais { get; set; }
        public string? codigoPostal { get; set; }
        public int PersonaId { get; set; }
    }
}
