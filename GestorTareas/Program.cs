using System.Text;
using GestorTareas.Data;
using GestorTareas.Domain;
using GestorTareas.Infrastructure.Middleware;
using GestorTareas.Services;
using GestorTareas.Services.Auth;
using GestorTareas.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

if (args.Contains("console"))
{
    Console.Title = "Gestor de Tareas (Modo consola)";

    string consoleConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=GestorTareas;Trusted_Connection=True;TrustServerCertificate=True;";

    var config = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(consoleConnectionString)
        .Options;

    var context = new AppDbContext(options);
    context.Database.EnsureCreated();
    SeedUsuarios(context);

    var consoleJwtSettings = config.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
    var authService = new AuthService(context, Microsoft.Extensions.Options.Options.Create(consoleJwtSettings));
    ITareaRepository repo = new EfTareaRepository(context);
    TareaService servicio = new TareaService(repo, context);

    var ui = new ConsoleUI(servicio, authService);
    ui.Ejecutar();

    return;
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ITareaRepository, EfTareaRepository>();
builder.Services.AddScoped<TareaService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
    SeedUsuarios(dbContext);
}

app.UseErrorHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void SeedUsuarios(AppDbContext dbContext)
{
    // Migrar passwords en texto plano a BCrypt (para BD existentes)
    var usuariosSinHash = dbContext.Usuarios
        .Where(u => u.Password != null && !u.Password.StartsWith("$2"))
        .ToList();

    foreach (var u in usuariosSinHash)
    {
        u.Password = BCrypt.Net.BCrypt.HashPassword(u.Password);
    }

    if (usuariosSinHash.Count > 0)
        dbContext.SaveChanges();

    // Seed inicial si no hay usuarios
    if (!dbContext.Usuarios.Any())
    {
        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var pedroId = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var mariaId = Guid.Parse("44444444-4444-4444-4444-444444444444");

        dbContext.Usuarios.AddRange(
            new Usuario
            {
                Id = adminId,
                Nombre = "admin",
                Password = BCrypt.Net.BCrypt.HashPassword("admin"),
                Rol = "admin"
            },
            new Usuario
            {
                Id = pedroId,
                Nombre = "pedro",
                Password = BCrypt.Net.BCrypt.HashPassword("pedro"),
                Rol = "user"
            },
            new Usuario
            {
                Id = mariaId,
                Nombre = "maria",
                Password = BCrypt.Net.BCrypt.HashPassword("maria"),
                Rol = "user"
            }
        );

        dbContext.SaveChanges();

        dbContext.Tareas.AddRange(
            new TareaSimple("Comprar pan", "Ir a la panadería antes de las 10", DateTime.Now.AddDays(1), PrioridadTarea.Media)
            {
                Id = Guid.NewGuid(),
                UsuarioId = pedroId,
                FechaCreacion = DateTime.Now
            },
            new TareaRecurrente("Regar plantas", "Regar todas las macetas", DateTime.Now.AddDays(3), PrioridadTarea.Baja, 3)
            {
                Id = Guid.NewGuid(),
                UsuarioId = pedroId,
                FechaCreacion = DateTime.Now
            },
            new TareaUrgente("Revisión médica", "Análisis de sangre", DateTime.Now.AddHours(5), PrioridadTarea.Alta, "Pedro")
            {
                Id = Guid.NewGuid(),
                UsuarioId = pedroId,
                FechaCreacion = DateTime.Now
            }
        );

        dbContext.Tareas.AddRange(
            new TareaSimple("Estudiar inglés", "Lección 5 del curso", DateTime.Now.AddDays(2), PrioridadTarea.Alta)
            {
                Id = Guid.NewGuid(),
                UsuarioId = mariaId,
                FechaCreacion = DateTime.Now
            },
            new TareaRecurrente("Hacer yoga", "Sesión de 30 min", DateTime.Now.AddDays(1), PrioridadTarea.Baja, 2)
            {
                Id = Guid.NewGuid(),
                UsuarioId = mariaId,
                FechaCreacion = DateTime.Now
            },
            new TareaUrgente("Presentar informe", "Entregar informe trimestral", DateTime.Now.AddHours(8), PrioridadTarea.Alta, "María")
            {
                Id = Guid.NewGuid(),
                UsuarioId = mariaId,
                FechaCreacion = DateTime.Now
            }
        );

        dbContext.SaveChanges();
    }
}
