using GestorTareas.Domain;

namespace GestorTareas.Data
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = "user";

        public List<Tarea> Tareas { get; set; } = new();
    }
}