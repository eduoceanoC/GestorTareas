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

        public void Actualizar(Tarea tarea)
        {
            _context.Tareas.Update(tarea);
            _context.SaveChanges();
        }

        public void Eliminar(Guid id)
        {
            var tarea = _context.Tareas.Find(id);

            if (tarea != null)
            {
                _context.Tareas.Remove(tarea);

                var stat = _context.Estadisticas
                    .FirstOrDefault(s => s.Clave == "TareasEliminadas");

                if (stat == null)
                {
                    stat = new Estadistica { Clave = "TareasEliminadas", Valor = 0 };
                    _context.Estadisticas.Add(stat);
                }

                stat.Valor++;
                _context.SaveChanges();
            }
        }

        public Tarea? ObtenerPorId(Guid id)
        {
            return _context.Tareas
                .Include(t => t.Usuario)
                .FirstOrDefault(t => t.Id == id);
        }

        public List<Tarea> ObtenerTodas()
        {
            return _context.Tareas
                .Include(t => t.Usuario)
                .AsNoTracking()
                .OrderBy(t => t.FechaCreacion)
                .ToList();
        }

        public List<Tarea> ObtenerPorUsuario(Guid usuarioId)
        {
            return _context.Tareas
                .Include(t => t.Usuario)
                .AsNoTracking()
                .Where(t => t.UsuarioId == usuarioId)
                .OrderBy(t => t.FechaCreacion)
                .ToList();
        }

        public List<Tarea> BuscarPorEstado(EstadoTarea estado)
        {
            return _context.Tareas
                .Include(t => t.Usuario)
                .Where(t => t.Estado == estado)
                .ToList();
        }

        public List<Tarea> BuscarPorUsuarioYEstado(Guid usuarioId, EstadoTarea estado)
        {
            return _context.Tareas
                .Include(t => t.Usuario)
                .Where(t => t.UsuarioId == usuarioId && t.Estado == estado)
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
    }
}
