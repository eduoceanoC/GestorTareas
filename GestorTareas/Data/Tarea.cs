using System;
using GestorTareas.Data;

namespace GestorTareas.Domain
{
    public abstract class Tarea
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
        public string Titulo { get; set; }
        public string? Descripcion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaLimite { get; set; }
        public PrioridadTarea Prioridad { get; set; }

        private EstadoTarea _estado;
        public EstadoTarea Estado { get => _estado; private set => _estado = value; }
        public string? MotivoCancelacion { get; set; }

        protected Tarea() { }

        protected Tarea(string titulo, string descripcion, DateTime fechaLimite, PrioridadTarea prioridad)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                throw new ArgumentException("El título no puede estar vacío");            

            Id = Guid.NewGuid();
            Titulo = titulo.Trim();
            Descripcion = descripcion ?? string.Empty;
            FechaCreacion = DateTime.Now;
            FechaLimite = fechaLimite;
            Prioridad = prioridad;
            _estado = EstadoTarea.Pendiente;
            MotivoCancelacion = null;
        }

        public bool Iniciar()
        {
            if (_estado != EstadoTarea.Pendiente) return false;
            _estado = EstadoTarea.EnProgreso;
            return true;
        }

        public bool Completar()
        {
            if (_estado == EstadoTarea.Completada || _estado == EstadoTarea.Cancelada) return false;
            _estado = EstadoTarea.Completada;
            return true;
        }

        public bool Cancelar(string motivo)
        {
            if (_estado == EstadoTarea.Cancelada) return false;
            _estado = EstadoTarea.Cancelada;
            MotivoCancelacion = motivo ?? "Sin especificar";
            return true;
        }

        public virtual bool EstaVencida => DateTime.Now > FechaLimite &&
                                           _estado != EstadoTarea.Completada &&
                                           _estado != EstadoTarea.Cancelada;

        public virtual void Resumen()
        {
            Console.WriteLine($"ID: {Id}");
            Console.WriteLine($"Título: {Titulo}");
            Console.WriteLine($"Descripción: {Descripcion}");
            Console.WriteLine($"Estado: {_estado}");
            Console.WriteLine($"Prioridad: {Prioridad}");
            Console.WriteLine($"Fecha límite: {FechaLimite:dd/MM/yyyy}");
            Console.WriteLine($"Vencida: {(EstaVencida ? "Sí" : "No")}");
        }
    }
}