using System;

namespace GestorTareas.Domain
{
    public class TareaRecurrente : Tarea
    {
        protected TareaRecurrente() { }
        public int IntervaloEnDias { get; set; }

        public TareaRecurrente(string titulo, string descripcion, DateTime fechaLimite,
                               PrioridadTarea prioridad, int intervaloEnDias)
            : base(titulo, descripcion, fechaLimite, prioridad)
        {
            if (intervaloEnDias <= 0)
                throw new ArgumentException("El intervalo debe ser mayor que cero");

            IntervaloEnDias = intervaloEnDias;
        }

        public TareaRecurrente GenerarSiguiente()
        {
            return new TareaRecurrente(Titulo, Descripcion,
                                       FechaLimite.AddDays(IntervaloEnDias),
                                       Prioridad, IntervaloEnDias);
        }

        public override void Resumen()
        {
            Console.WriteLine($"[RECURRENTE cada {IntervaloEnDias}d] {Titulo}");
            Console.WriteLine($"   Descripción: {Descripcion}");
            Console.WriteLine($"   Límite: {FechaLimite:dd/MM/yyyy} | Estado: {Estado}");
            if (EstaVencida)
                Console.WriteLine($"   VENCIDA");
        }
    }
}