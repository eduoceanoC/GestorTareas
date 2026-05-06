using System;
using System.Collections.Generic;
using GestorTareas.Domain;

namespace GestorTareas.Data
{
    public interface ITareaRepository
    {
        void Agregar(Tarea tarea);
        void Actualizar(Tarea tarea);
        void Eliminar(Guid id);
        Tarea? ObtenerPorId(Guid id);
        List<Tarea> ObtenerTodas();
        List<Tarea> ObtenerPorUsuario(Guid usuarioId);
        List<Tarea> BuscarPorEstado(EstadoTarea estado);
        List<Tarea> BuscarPorUsuarioYEstado(Guid usuarioId, EstadoTarea estado);
        int ContarEliminadas { get; }
    }
}
