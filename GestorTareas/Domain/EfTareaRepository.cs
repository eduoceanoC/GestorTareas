using GestorTareas.Data;
using GestorTareas.Domain;
using Microsoft.EntityFrameworkCore;

namespace GestorTareas.Data
{
    public class EfTareaRepository : ITareaRepository
    {
        private readonly AppDbContext _context;

        public EfTareaRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Agregar(Tarea tarea)
        {
            _context.Tareas.Add(tarea);
            _context.SaveChanges();
        }
        public List<Tarea> ObtenerTodas()
        {
            return _context.Tareas
                .AsNoTracking()
                .ToList();
        }
        public void Actualizar(Tarea tarea)
        {
            _context.Tareas.Update(tarea);
            _context.SaveChanges();
        }
        public Tarea ObtenerPorId(Guid id)
        {
            return _context.Tareas.FirstOrDefault(t => t.Id == id);
        }

        public void Eliminar(Guid id)
        {
            var tarea = _context.Tareas.Find(id);

            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);
                
                var stat = _context.Estadisticas
                    .FirstOrDefault(s => s.Clave == "TareasEliminadas");

                if (stat != null)
                {
                    stat.Valor++;
                }

                _context.SaveChanges();
            }
        }

        public List<Tarea> BuscarPorEstado(EstadoTarea estado)
        {
            return _context.Tareas
                .Where(t => t.Estado == estado)
                .ToList();
        }

        public int ContarEliminadas
        {
            get
            {
                var stat = _context.Estadisticas
                    .FirstOrDefault(s => s.Clave == "TareasEliminadas");

                return stat?.Valor ?? 0;
            }
        }

        public void IncrementarEliminadas() { }
    }
}