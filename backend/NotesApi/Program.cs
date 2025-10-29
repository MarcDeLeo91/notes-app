using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Data;
using NotesApi.Services;
using Microsoft.OpenApi.Models;
using NotesApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var logger = loggerFactory.CreateLogger("Program");

// Configuración de DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=notes.db";

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(connectionString));

// Configuración de JWT Auth
var keyBase64 = builder.Configuration["Jwt:KeyBase64"];
byte[] signingKeyBytes;
if (!string.IsNullOrEmpty(keyBase64))
{
    try
    {
        signingKeyBytes = Convert.FromBase64String(keyBase64);
    }
    catch (FormatException)
    {
        logger.LogError("Jwt:KeyBase64 is not a valid base64 string.");
        throw new Exception("Jwt:KeyBase64 is not a valid base64 string.");
    }
}
else
{
    var key = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(key))
    {
        logger.LogError("Jwt signing key missing. Set 'Jwt:KeyBase64' or 'Jwt:Key' in configuration.");
        throw new Exception("Jwt signing key missing. Set 'Jwt:KeyBase64' or 'Jwt:Key' in configuration.");
    }
    signingKeyBytes = Encoding.UTF8.GetBytes(key);
}

if (signingKeyBytes.Length * 8 <= 256)
{
    logger.LogError("Configured JWT key is too short. Use a key longer than 256 bits (recommended 512 bits).");
    throw new Exception("Configured JWT key is too short. Use a key longer than 256 bits (recommended 512 bits).");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
        ClockSkew = TimeSpan.FromMinutes(5)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            logger.LogWarning($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            logger.LogInformation("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});

// Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000") // Agrega el puerto correcto aquí
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Inyección de dependencias (DI) - servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<PlannerService>();
builder.Services.AddScoped<ExecutorService>();
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<ProfesorService>();
builder.Services.AddScoped<CursoService>();
builder.Services.AddScoped<InscripcionService>();
builder.Services.AddScoped<NotaService>();

// Configuración de servicios en segundo plano
builder.Services.AddHostedService<AgentWorker>();

// Configuración de controladores y Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NotesApi", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese el token JWT en el campo: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Aplicar migraciones y seed inicial
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.Migrate();

    if (!ctx.Profesores.Any())
    {
        var p1 = new Profesor { Nombre = "Ana", Apellido = "Lopez", Email = "ana@uni.edu", Departamento = "Matemáticas" };
        var p2 = new Profesor { Nombre = "Carlos", Apellido = "Ramirez", Email = "carlos@uni.edu", Departamento = "Física" };
        ctx.Profesores.AddRange(p1, p2);
        ctx.SaveChanges();

        var c1 = new Curso { Nombre = "Álgebra I", Codigo = "MAT101", Creditos = 4, ProfesorId = p1.Id };
        var c2 = new Curso { Nombre = "Física I", Codigo = "FIS101", Creditos = 3, ProfesorId = p2.Id };
        ctx.Cursos.AddRange(c1, c2);
        ctx.SaveChanges();

        var e1 = new Estudiante { Nombre = "Juan", Apellido = "Perez", Email = "juan@ejemplo.com", FechaNacimiento = new DateTime(2000, 1, 1) };
        var e2 = new Estudiante { Nombre = "Luisa", Apellido = "Martinez", Email = "luisa@ejemplo.com", FechaNacimiento = new DateTime(2001, 5, 12) };
        ctx.Estudiantes.AddRange(e1, e2);
        ctx.SaveChanges();

        var ins1 = new Inscripcion { EstudianteId = e1.Id, CursoId = c1.Id };
        var ins2 = new Inscripcion { EstudianteId = e2.Id, CursoId = c1.Id };
        var ins3 = new Inscripcion { EstudianteId = e1.Id, CursoId = c2.Id };
        ctx.Inscripciones.AddRange(ins1, ins2, ins3);
        ctx.SaveChanges();

        ctx.Notas.AddRange(
            new NotaAcademica { InscripcionId = ins1.Id, Valor = 4.5m },
            new NotaAcademica { InscripcionId = ins2.Id, Valor = 3.8m },
            new NotaAcademica { InscripcionId = ins3.Id, Valor = 4.0m }
        );
        ctx.SaveChanges();

        logger.LogInformation("Seed inicial aplicado: profesores, cursos, estudiantes, inscripciones, notas.");
    }
}

// Middleware
app.UseRouting();
app.UseCors("AllowLocalDev");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotesApi v1");
    });
}

app.MapControllers();

logger.LogInformation("La aplicación ha iniciado correctamente.");
app.Run();