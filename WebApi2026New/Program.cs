
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;
using WebApi2026New.Data;
using WebApi2026New.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// 1. CONFIGURACIÓN DE BASE DE DATOS
// ========================================

builder.Services.AddDbContext<AppDBContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
});

// ========================================
// 2. CONFIGURACIÓN DE JWT
// ========================================

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt")
);

var jwtSettings = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "No se encontró la configuración JWT."
    );

// Validar que la clave JWT sea segura
if (string.IsNullOrWhiteSpace(jwtSettings.Key) ||
    Encoding.UTF8.GetByteCount(jwtSettings.Key) < 32)
{
    throw new InvalidOperationException(
        "La clave JWT debe tener al menos 32 bytes."
    );
}

// Registrar JwtHelper para generar tokens
builder.Services.AddScoped<JwtHelper>();

// ========================================
// 3. AUTENTICACIÓN JWT BEARER
// ========================================

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Mantener los nombres originales de los claims
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                // Permite que User.Identity.Name
                // obtenga el claim "name" del JWT
                NameClaimType = "name",

                // Validar el emisor del token
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                // Validar la audiencia del token
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                // Validar la fecha de expiración
                ValidateLifetime = true,

                // Validar la firma digital
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)
                ),

                // No permitir tiempo adicional
                // después de la expiración
                ClockSkew = TimeSpan.Zero
            };
    });

// ========================================
// 4. AUTORIZACIÓN Y CONTROLADORES
// ========================================

// Habilitar autorización mediante [Authorize]
builder.Services.AddAuthorization();

// Registrar controladores de la API
builder.Services.AddControllers();

// ========================================
// 5. OPENAPI Y SCALAR
// ========================================

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer(
        (document, context, cancellationToken) =>
        {
            document.Components ??= new OpenApiComponents();

            document.Components.SecuritySchemes ??=
                new Dictionary<string, IOpenApiSecurityScheme>();

            // Configurar esquema Bearer JWT
            document.Components.SecuritySchemes["Bearer"] =
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Introduce tu token JWT."
                };

            return Task.CompletedTask;
        });
});

var app = builder.Build();

// ========================================
// 6. CONFIGURACIÓN DE SCALAR
// ========================================

if (app.Environment.IsDevelopment())
{
    // Generar documentación OpenAPI
    app.MapOpenApi();

    // Habilitar interfaz Scalar con Bearer JWT
    app.MapScalarApiReference(options =>
    {
        options.AddPreferredSecuritySchemes("Bearer");
    });
}

// ========================================
// 7. CONFIGURACIÓN DE MIDDLEWARE
// ========================================

// Redireccionar HTTP a HTTPS
app.UseHttpsRedirection();

// Primero verificar la identidad del usuario
app.UseAuthentication();

// Después comprobar la autorización
app.UseAuthorization();

// Habilitar endpoints de los controladores
app.MapControllers();

// Iniciar la aplicación
app.Run();
