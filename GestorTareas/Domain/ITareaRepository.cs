using System;
using System.Collections.Generic;
using GestorTareas.Domain;

namespace GestorTareas.Data
{
    public interface ITareaRepository
    {
        void Agregar(Tarea tarea);
        void Eliminar(Guid id);
        Tarea ObtenerPorId(Guid id);
        List<Tarea> ObtenerTodas();
        List<Tarea> BuscarPorEstado(EstadoTarea estado);
        int ContarEliminadas { get; }
    }
}