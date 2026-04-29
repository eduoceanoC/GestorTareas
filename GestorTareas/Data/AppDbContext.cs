using Microsoft.EntityFrameworkCore;
using GestorTareas.Domain;

namespace GestorTareas.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Estadistica> Estadisticas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tarea>(entity =>
            {
                entity.ToTable("Tareas");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Descripcion).HasMaxLength(int.MaxValue);
                entity.Property(e => e.FechaCreacion).IsRequired();
                entity.Property(e => e.FechaLimite).IsRequired();
                entity.Property(e => e.Prioridad).HasColumnType("tinyint").HasConversion<byte>();
                entity.Property(e => e.Estado)
                    .HasColumnType("tinyint")
                    .HasField("_estado")
                    .HasConversion<byte>()
                    .UsePropertyAccessMode(PropertyAccessMode.Field);
                entity.Property(e => e.MotivoCancelacion).HasMaxLength(500);
                entity.Property<int>("TipoTarea").HasColumnType("tinyint");

                // Discriminador
                entity.HasDiscriminator<int>("TipoTarea")
                    .HasValue<TareaSimple>(1)
                    .HasValue<TareaRecurrente>(2)
                    .HasValue<TareaUrgente>(3);
            });

            modelBuilder.Entity<TareaRecurrente>()
                .Property(e => e.IntervaloEnDias).HasColumnName("IntervaloEnDias");

            modelBuilder.Entity<TareaUrgente>()
                .Property(e => e.Responsable).HasMaxLength(100);


            modelBuilder.Entity<Estadistica>(entity =>
            {
                entity.ToTable("Estadisticas");
                entity.HasKey(e => e.Clave);
                entity.Property(e => e.Valor).IsRequired();
            });
        }
    }

    public class Estadistica
    {
        public string Clave { get; set; }
        public int Valor { get; set; }
    }
}