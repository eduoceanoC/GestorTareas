using System;

namespace GestorTareas.Domain
{
    public class TareaSimple : Tarea
    {
        public TareaSimple(string titulo, string descripcion, DateTime fechaLimite,
                           PrioridadTarea prioridad)
            : base(titulo, descripcion, fechaLimite, prioridad)
        { }

        public override void Resumen()
        {
            Console.WriteLine($"[SIMPLE] {Titulo}");
            Console.WriteLine($"   Descripción: {Descripcion}");
            Console.WriteLine($"   Límite: {FechaLimite:dd/MM/yyyy} | Prioridad: {Prioridad} | Estado: {Estado}");
            if (EstaVencida)
                Console.WriteLine($"   VENCIDA");
        }
    }
}