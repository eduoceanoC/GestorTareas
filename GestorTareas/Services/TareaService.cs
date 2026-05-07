using System;
using System.Collections.Generic;
using System.Linq;
using GestorTareas.Domain;
using GestorTareas.Data;
using GestorTareas.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GestorTareas.Services
{
    public class TareaService
    {
        private readonly ITareaRepository _repository;
        private readonly AppDbContext _context;
        private readonly ILogger<TareaService> _logger;

        public TareaService(ITareaRepository repository) : this(repository, null!, null!)
        {
        }

        public TareaService(ITareaRepository repository, AppDbContext context) : this(repository, context, null!)
        {
        }

        public TareaService(ITareaRepository repository, AppDbContext context, ILogger<TareaService> logger)
        {
            _repository = repository;
            _context = context;
            _logger = logger;
        }

        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            if (_context == null) return new List<Usuario>();
            return _context.Usuarios.AsNoTracking().ToList();
        }

        public void AgregarTarea(Tarea tarea, Guid usuarioId)
        {
            if (string.IsNullOrWhiteSpace(tarea.Titulo))
            {
                _logger?.LogWarning("Intento de crear tarea con título vacío por usuario {UsuarioId}", usuarioId);
                throw new ArgumentException("El título no puede estar vacío");
            }

            tarea.UsuarioId = usuarioId;
            _repository.Agregar(tarea);
            _logger?.LogInformation("Tarea '{Titulo}' (ID: {TareaId}) creada por usuario {UsuarioId}",
                tarea.Titulo, tarea.Id, usuarioId);
        }

        public List<Tarea> ObtenerTodas() => _repository.ObtenerTodas();

        public List<Tarea> ObtenerPorUsuario(Guid usuarioId) => _repository.ObtenerPorUsuario(usuarioId);

        public Tarea? ObtenerPorId(Guid id) => _repository.ObtenerPorId(id);

        public void GuardarCambios(Tarea tarea) => _repository.Actualizar(tarea);

        public void EliminarTarea(Guid id) => _repository.Eliminar(id);

        public void GenerarSiguienteRecurrente(TareaRecurrente recurrente)
        {
            var siguiente = recurrente.GenerarSiguiente();
            siguiente.UsuarioId = recurrente.UsuarioId;
            _repository.Agregar(siguiente);
        }

        public bool PuedeAdministrarTarea(Tarea tarea, Guid usuarioId, bool esAdmin)
        {
            return esAdmin || tarea.UsuarioId == usuarioId;
        }

        public PaginadoResponseDto<Tarea> ObtenerPaginado(int pagina, int porPagina, Guid? usuarioId = null, bool esAdmin = false)
        {
            var todas = esAdmin || usuarioId == null
                ? _repository.ObtenerTodas()
                : _repository.ObtenerPorUsuario(usuarioId.Value);

            var totalRegistros = todas.Count;

            var datos = todas
                .Skip((pagina - 1) * porPagina)
                .Take(porPagina)
                .ToList();

            var totalPaginas = totalRegistros == 0 ? 0 : (int)Math.Ceiling((double)totalRegistros / porPagina);

            return new PaginadoResponseDto<Tarea>
            {
                Datos = datos,
                TotalRegistros = totalRegistros,
                TotalPaginas = totalPaginas,
                HayPaginaAnterior = pagina > 1,
                HayPaginaSiguiente = pagina < totalPaginas
            };
        }

        public int TotalTareas => _repository.ObtenerTodas().Count;
        public int TareasPendientes => _repository.BuscarPorEstado(EstadoTarea.Pendiente).Count;
        public int TareasEnProgreso => _repository.BuscarPorEstado(EstadoTarea.EnProgreso).Count;
        public int TareasCompletadas => _repository.BuscarPorEstado(EstadoTarea.Completada).Count;
        public int TareasCanceladas => _repository.BuscarPorEstado(EstadoTarea.Cancelada).Count;
        public int TareasEliminadas => _repository.ContarEliminadas;

        public int TareasVencidas => _repository.ObtenerTodas().Count(t => t.EstaVencida);

        public int TareasPorPrioridad(PrioridadTarea p) => _repository.ObtenerTodas().Count(t => t.Prioridad == p);
        public int TareasSimples => _repository.ObtenerTodas().OfType<TareaSimple>().Count();
        public int TareasRecurrentes => _repository.ObtenerTodas().OfType<TareaRecurrente>().Count();
        public int TareasUrgentes => _repository.ObtenerTodas().OfType<TareaUrgente>().Count();
    }
}
