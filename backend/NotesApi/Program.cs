using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Data;
using NotesApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // Habilitar logs en la consola
builder.Logging.AddDebug(); // Habilitar logs para depuración

// Crear un logger manualmente
var loggerFactory = LoggerFactory.Create(logging => logging.AddConsole());
var logger = loggerFactory.CreateLogger("Program");

// Configuración de DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Asegurarse de que la clave tenga más de 256 bits
if (signingKeyBytes.Length * 8 <= 256)
{
    logger.LogError("Configured JWT key is too short. Use a key longer than 256 bits (recommended 512 bits).");
    throw new Exception("Configured JWT key is too short. Use a key longer than 256 bits (recommended 512 bits).");
}

// Configuración de autenticación JWT
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
        ClockSkew = TimeSpan.FromMinutes(5) // Permite un margen de 5 minutos para desajustes de tiempo
    };

    // Registrar eventos para depuración
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
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
              // .AllowCredentials(); // Descomentar si usas cookies/sesiones
    });
});

// Inyección de dependencias (DI) - servicios
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<PlannerService>();
builder.Services.AddScoped<ExecutorService>();

// Configuración de servicios en segundo plano
builder.Services.AddHostedService<AgentWorker>();

// Configuración de controladores y Swagger
builder.Services.AddControllers();
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

// Middleware
app.UseRouting();

// Aplicar CORS antes de autenticación/autorizar
app.UseCors("AllowLocalDev");

app.UseAuthentication();
app.UseAuthorization();

// Configuración de Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotesApi v1");
        // opcional: c.RoutePrefix = string.Empty; // para servir Swagger en la raíz
    });
}

// Mapear controladores
app.MapControllers();

logger.LogInformation("La aplicación ha iniciado correctamente.");
app.Run();