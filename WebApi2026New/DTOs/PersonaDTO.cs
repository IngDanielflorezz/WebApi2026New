namespace WebApi2026New.DTOs
{
    public class PersonaDTO
    {
        public int idPersona { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;

        public List<DireccionDTO> direcciones { get; set; } = new();
    }

    public class PersonaCreateDTO
    {

        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public string correo { get; set; } = string.Empty;
        public int edad { get; set; }
        public string password { get; set; } = string.Empty;

    }

    public class PersonaReadDTO
    {
        public int idPersona { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public int edad { get; set; }
        public string correo { get; set; } = string.Empty;
    }

    public class PersonaListaDto
    {
        public int idPersona { get; set; }
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public int edad { get; set; }
        public string correo { get; set; } = string.Empty;
        public int totalDirecciones { get; set; }
    }

    public class PersonaUpdateDTO
    {
        public string nombre { get; set; } = string.Empty;
        public string apellido { get; set; } = string.Empty;
        public int edad { get; set; }
    }

}
