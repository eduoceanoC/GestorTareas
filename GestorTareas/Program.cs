using Microsoft.EntityFrameworkCore;
using GestorTareas.Data;
using GestorTareas.Services;
using GestorTareas.UI;


if (args.Contains("console"))
{
    Console.Title = "Gestor de Tareas (Modo consola)";

    string consoleConnectionString =
        "Server=(localdb)\\MSSQLLocalDB;Database=GestorTareas;Trusted_Connection=True;TrustServerCertificate=True;";

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer(consoleConnectionString)
        .Options;

    var context = new AppDbContext(options);

    ITareaRepository repo = new EfTareaRepository(context);
    TareaService servicio = new TareaService(repo);

    var ui = new ConsoleUI(servicio);
    ui.Ejecutar();

    return;
}

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ITareaRepository, EfTareaRepository>();
builder.Services.AddScoped<TareaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();