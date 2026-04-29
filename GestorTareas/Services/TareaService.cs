using System;
using System.Collections.Generic;
using System.Linq;
using GestorTareas.Domain;
using GestorTareas.Data;

namespace GestorTareas.Services
{
    public class TareaService
    {
        private readonly ITareaRepository _repository;

        public TareaService(ITareaRepository repository)
        {
            _repository = repository;
        }

        public void AgregarTarea(Tarea tarea) => _repository.Agregar(tarea);

        public List<Tarea> ObtenerTodas() => _repository.ObtenerTodas();

        public Tarea ObtenerPorId(Guid id) => _repository.ObtenerPorId(id);

        public void EliminarTarea(Guid id) => _repository.Eliminar(id);

        public void GenerarSiguienteRecurrente(TareaRecurrente recurrente)
        {
            var siguiente = recurrente.GenerarSiguiente();
            _repository.Agregar(siguiente);
        }

        // Estadísticas
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