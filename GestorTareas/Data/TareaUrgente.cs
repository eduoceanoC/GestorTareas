using System;

namespace GestorTareas.Domain
{
    public class TareaUrgente : Tarea
    {
        public string? Responsable { get; set; }

        protected TareaUrgente() { }

        public TareaUrgente(string titulo, string descripcion, DateTime fechaLimite,
                            PrioridadTarea prioridad, string responsable)
            : base(titulo, descripcion, fechaLimite, prioridad)
        {
            if (string.IsNullOrWhiteSpace(responsable))
                throw new ArgumentException("Una tarea urgente debe tener responsable");

            Responsable = responsable;
        }

        public override bool EstaVencida =>
            Estado != EstadoTarea.Completada &&
            Estado != EstadoTarea.Cancelada &&
            DateTime.Now > FechaLimite;

        public override void Resumen()
        {
            Console.WriteLine($"[URGENTE] {Titulo}");
            Console.WriteLine($"   Responsable: {Responsable}");
            Console.WriteLine($"   Límite: {FechaLimite:dd/MM/yyyy HH:mm} | Estado: {Estado}");
        }
    }
}