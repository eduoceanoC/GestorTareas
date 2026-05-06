using System;
using GestorTareas.Domain;

public class CrearTareaDto
{
    public string Titulo { get; set; }
    public string? Descripcion { get; set; }
    public DateTime FechaLimite { get; set; }
    public PrioridadTarea Prioridad { get; set; }
    public int Tipo { get; set; }
    public int? IntervaloEnDias { get; set; }
    public string? Responsable { get; set; }
}
