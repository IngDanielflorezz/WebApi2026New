
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi2026New.Data;
using WebApi2026New.DTOs;
using WebApi2026New.Models;

// Proteger todos los endpoints mediante JWT
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DireccionesController : ControllerBase
{
    private readonly AppDBContext _context;

    public DireccionesController(AppDBContext context)
    {
        _context = context;
    }

    // GET: api/Direcciones
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DireccionReadDTO>>> GetDirecciones()
    {
        return await _context.Direcciones
            .Select(d => new DireccionReadDTO
            {
                idDireccion = d.idDireccion,
                calle = d.calle,
                ciudad = d.ciudad,
                departamento = d.departamento,
                pais = d.pais,
                codigoPostal = d.codigoPostal,
                PersonaId = d.PersonaId
            })
            .ToListAsync();
    }

    // GET: api/Direcciones/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DireccionReadDTO>> GetDireccion(int id)
    {
        var direccion = await _context.Direcciones.FindAsync(id);

        if (direccion == null)
        {
            return NotFound();
        }

        return ToReadDTO(direccion);
    }

    // GET: api/Direcciones/persona/5
    [HttpGet("persona/{personaId}")]
    public async Task<ActionResult<IEnumerable<DireccionReadDTO>>> GetDireccionesPorPersona(
        int personaId)
    {
        if (!await _context.Personas.AnyAsync(
            p => p.idPersona == personaId))
        {
            return NotFound("La persona no existe.");
        }

        return await _context.Direcciones
            .Where(d => d.PersonaId == personaId)
            .Select(d => new DireccionReadDTO
            {
                idDireccion = d.idDireccion,
                calle = d.calle,
                ciudad = d.ciudad,
                departamento = d.departamento,
                pais = d.pais,
                codigoPostal = d.codigoPostal,
                PersonaId = d.PersonaId
            })
            .ToListAsync();
    }

    // POST: api/Direcciones
    [HttpPost]
    public async Task<ActionResult<DireccionReadDTO>> PostDireccion(
        DireccionCreateDTO dto)
    {
        // Verificar que la persona exista
        if (!await _context.Personas.AnyAsync(
            p => p.idPersona == dto.PersonaId))
        {
            return BadRequest("La persona no existe.");
        }

        var direccion = new Direccion
        {
            calle = dto.calle,
            ciudad = dto.ciudad,
            departamento = dto.departamento,
            pais = dto.pais,
            codigoPostal = dto.codigoPostal,
            PersonaId = dto.PersonaId
        };

        _context.Direcciones.Add(direccion);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetDireccion),
            new { id = direccion.idDireccion },
            ToReadDTO(direccion)
        );
    }

    // PUT: api/Direcciones/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDireccion(
        int id, DireccionUpdateDTO dto)
    {
        var direccion = await _context.Direcciones.FindAsync(id);

        if (direccion == null)
        {
            return NotFound();
        }

        // Actualizar únicamente los campos permitidos
        direccion.calle = dto.calle;
        direccion.ciudad = dto.ciudad;
        direccion.departamento = dto.departamento;
        direccion.pais = dto.pais;
        direccion.codigoPostal = dto.codigoPostal;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Direcciones/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDireccion(int id)
    {
        var direccion = await _context.Direcciones.FindAsync(id);

        if (direccion == null)
        {
            return NotFound();
        }

        _context.Direcciones.Remove(direccion);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Convertir una entidad Direccion a DireccionReadDTO
    private static DireccionReadDTO ToReadDTO(Direccion d)
    {
        return new DireccionReadDTO
        {
            idDireccion = d.idDireccion,
            calle = d.calle,
            ciudad = d.ciudad,
            departamento = d.departamento,
            pais = d.pais,
            codigoPostal = d.codigoPostal,
            PersonaId = d.PersonaId
        };
    }
}
