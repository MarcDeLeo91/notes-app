using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NotesApi.Data;
using NotesApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Auth
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
        throw new Exception("Jwt:KeyBase64 is not a valid base64 string.");
    }
}
else
{
    var key = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(key)) throw new Exception("Jwt signing key missing. Set 'Jwt:KeyBase64' or 'Jwt:Key' in configuration.");
    signingKeyBytes = Encoding.UTF8.GetBytes(key);
}

// Ensure key length > 256 bits
if (signingKeyBytes.Length * 8 <= 256)
    throw new Exception("Configured JWT key is too short. Use a key longer than 256 bits (recommended 512 bits).");

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
        ClockSkew = TimeSpan.Zero // Reduce tolerancia de tiempo para tokens expirados
    };

    // Opcional: Registrar eventos para depuración
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully.");
            return Task.CompletedTask;
        }
    };
});

// CORS
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

// DI - services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<NoteService>();
builder.Services.AddScoped<AiService>();
builder.Services.AddScoped<PlannerService>();
builder.Services.AddScoped<ExecutorService>();

// Worker
builder.Services.AddHostedService<AgentWorker>();

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

// Asegurarse de que Swagger esté activado
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NotesApi v1");
    // opcional: c.RoutePrefix = string.Empty; // para servir Swagger en la raíz
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();