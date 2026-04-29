using System;
using System.Linq;
using GestorTareas.Domain;
using GestorTareas.Services;

namespace GestorTareas.UI
{
    public class ConsoleUI
    {
        private readonly TareaService _service;
        private int _contadorSeleccion;

        public ConsoleUI(TareaService service)
        {
            _service = service;
            _contadorSeleccion = 1;
        }

        public void Ejecutar()
        {
            bool salir = false;
            while (!salir)
            {
                MostrarMenuPrincipal();
                string opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1": AgregarTarea(); break;
                    case "2": ListarTareas(); break;
                    case "3": EditarTarea(); break;
                    case "4": EliminarTarea(); break;
                    case "5": MostrarEstadisticas(); break;
                    case "6": CambiarEstadoTarea(); break;
                    case "0":
                        salir = true;
                        Console.WriteLine("¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("Opción no válida. Presione Enter para continuar...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private void MostrarMenuPrincipal()
        {
            Console.Clear();
            Console.WriteLine("=== GESTOR DE TAREAS ===");
            Console.WriteLine("1. Agregar nueva tarea");
            Console.WriteLine("2. Listar todas las tareas");
            Console.WriteLine("3. Editar tarea");
            Console.WriteLine("4. Eliminar tarea");
            Console.WriteLine("5. Mostrar estadísticas");
            Console.WriteLine("6. Cambiar estado de tarea");
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opción: ");
        }

        private void AgregarTarea()
        {
            Console.Clear();
            Console.WriteLine("=== AGREGAR NUEVA TAREA ===");
            Console.WriteLine("Tipos de tarea:");
            Console.WriteLine("1. Tarea Simple");
            Console.WriteLine("2. Tarea Recurrente");
            Console.WriteLine("3. Tarea Urgente");
            Console.Write("Seleccione tipo: ");
            string tipo = Console.ReadLine();

            Console.Write("Título: ");
            string titulo = Console.ReadLine();
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine();
            Console.Write("Prioridad (1=Baja, 2=Media, 3=Alta): ");
            string prioridadInput = Console.ReadLine();

            if (!int.TryParse(prioridadInput, out int prioridadIdx) || prioridadIdx < 1 || prioridadIdx > 3)
            {
                Console.WriteLine("Prioridad no válida. Operación cancelada. Presione Enter para continuar...");
                Console.ReadLine();
                return;
            }
            PrioridadTarea prioridad = (PrioridadTarea)(prioridadIdx - 1);

            try
            {
                switch (tipo)
                {
                    case "1":
                        Console.Write("Fecha límite (dd/MM/yyyy): ");
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime fechaLimite))
                        {
                            Console.WriteLine("Fecha inválida. Operación cancelada.");
                            break;
                        }
                        _service.AgregarTarea(new TareaSimple(titulo, descripcion, fechaLimite, prioridad));
                        break;

                    case "2":
                        Console.Write("Fecha límite (dd/MM/yyyy): ");
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out fechaLimite))
                        {
                            Console.WriteLine("Fecha inválida. Operación cancelada.");
                            break;
                        }
                        Console.Write("Intervalo en días: ");
                        if (!int.TryParse(Console.ReadLine(), out int intervalo) || intervalo <= 0)
                        {
                            Console.WriteLine("Intervalo inválido. Operación cancelada.");
                            break;
                        }
                        _service.AgregarTarea(new TareaRecurrente(titulo, descripcion, fechaLimite, prioridad, intervalo));
                        break;

                    case "3":
                        Console.Write("Fecha límite (dd/MM/yyyy HH:mm): ");
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime fechaHora))
                        {
                            Console.WriteLine("Fecha inválida. Operación cancelada.");
                            break;
                        }
                        Console.Write("Responsable: ");
                        string responsable = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(responsable))
                        {
                            Console.WriteLine("Responsable inválido. Operación cancelada.");
                            break;
                        }
                        _service.AgregarTarea(new TareaUrgente(titulo, descripcion, fechaHora, prioridad, responsable));
                        break;

                    default:
                        Console.WriteLine("Tipo no válido");
                        break;
                }

                Console.WriteLine("\n ✓ Tarea agregada exitosamente!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n ✗ Error al crear tarea: {ex.Message}");
            }

            Console.WriteLine("\n Presione Enter para continuar...");
            Console.ReadLine();
        }

        private void ListarTareas()
        {
            Console.Clear();
            Console.WriteLine("=== LISTA DE TAREAS ===\n");

            var tareas = _service.ObtenerTodas();
            foreach (var t in tareas)
            {
                t.Resumen();
            }

            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        private void EditarTarea()
        {
            var tareas = _service.ObtenerTodas();
            if (!ValidarTareasExistentes(tareas)) return;

            ListarTareasConIndices(tareas);

            Console.Write("\nSeleccione el número de tarea a editar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            Tarea tarea = tareas[indice - 1];
            Console.Clear();
            Console.WriteLine($"=== EDITANDO TAREA: {tarea.Titulo} ===\n");

            Console.Write($"Nuevo título (actual: {tarea.Titulo}): ");
            string nuevoTitulo = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevoTitulo)) tarea.Titulo = nuevoTitulo;

            Console.Write($"Nueva descripción (actual: {tarea.Descripcion}): ");
            string nuevaDesc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(nuevaDesc)) tarea.Descripcion = nuevaDesc;

            Console.Write($"Nueva prioridad (1=Baja,2=Media,3=Alta) (actual: {tarea.Prioridad}): ");
            string nuevaPrioridad = Console.ReadLine();
            if (int.TryParse(nuevaPrioridad, out int prioridadVal))
                tarea.Prioridad = (PrioridadTarea)(prioridadVal - 1);

            if (tarea is TareaRecurrente recurrente)
            {
                Console.Write($"Nuevo intervalo en días (actual: {recurrente.IntervaloEnDias}): ");
                string nuevoIntervalo = Console.ReadLine();
                if (int.TryParse(nuevoIntervalo, out int intervaloVal) && intervaloVal > 0)
                    recurrente.IntervaloEnDias = intervaloVal;
            }
            else if (tarea is TareaUrgente urgente)
            {
                Console.Write($"Nuevo responsable (actual: {urgente.Responsable}): ");
                string nuevoResp = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(nuevoResp)) urgente.Responsable = nuevoResp;
            }

            Console.WriteLine("\n✓ Tarea actualizada exitosamente!");
            Console.ReadLine();
        }

        private void EliminarTarea()
        {
            var tareas = _service.ObtenerTodas();
            if (!ValidarTareasExistentes(tareas)) return;

            ListarTareasConIndices(tareas);

            Console.Write("\nSeleccione el número de tarea a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            Tarea tarea = tareas[indice - 1];
            Console.Write($"¿Está seguro de eliminar '{tarea.Titulo}'? (S/N): ");
            if (Console.ReadLine().ToUpper() == "S")
            {
                _service.EliminarTarea(tarea.Id);
                Console.WriteLine("✓ Tarea eliminada exitosamente!");
            }
            Console.ReadLine();
        }

        private void CambiarEstadoTarea()
        {
            var tareas = _service.ObtenerTodas();
            if (!ValidarTareasExistentes(tareas)) return;

            ListarTareasConIndices(tareas);

            Console.Write("\nSeleccione el número de tarea: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            Tarea tarea = tareas[indice - 1];
            Console.Clear();
            Console.WriteLine($"=== CAMBIAR ESTADO: {tarea.Titulo} ===");
            Console.WriteLine($"Estado actual: {tarea.Estado}");
            Console.WriteLine("\nNuevo estado:");
            Console.WriteLine("1. Iniciar (Pendiente → EnProgreso)");
            Console.WriteLine("2. Completar");
            Console.WriteLine("3. Cancelar");
            Console.Write("Seleccione: ");

            string opcion = Console.ReadLine();
            bool exito = false;

            switch (opcion)
            {
                case "1": exito = tarea.Iniciar(); break;
                case "2": exito = tarea.Completar(); break;
                case "3":
                    Console.Write("Motivo de cancelación: ");
                    string motivo = Console.ReadLine();
                    exito = tarea.Cancelar(motivo);
                    break;
            }

            Console.WriteLine(exito ? "\n✓ Estado actualizado!" : "\n✗ No se pudo cambiar el estado");
            Console.ReadLine();
        }

        private void MostrarEstadisticas()
        {
            Console.Clear();
            Console.WriteLine("=== ESTADÍSTICAS ===\n");

            Console.WriteLine($"Total tareas: {_service.TotalTareas}");
            Console.WriteLine($"├─ Pendientes: {_service.TareasPendientes}");
            Console.WriteLine($"├─ En progreso: {_service.TareasEnProgreso}");
            Console.WriteLine($"├─ Completadas: {_service.TareasCompletadas}");
            Console.WriteLine($"└─ Canceladas: {_service.TareasCanceladas}");
            Console.WriteLine($"\nTareas vencidas: {_service.TareasVencidas}");
            Console.WriteLine($"Tareas eliminadas: {_service.TareasEliminadas}");

            Console.WriteLine("\n--- Por prioridad ---");
            Console.WriteLine($"Alta: {_service.TareasPorPrioridad(PrioridadTarea.Alta)}");
            Console.WriteLine($"Media: {_service.TareasPorPrioridad(PrioridadTarea.Media)}");
            Console.WriteLine($"Baja: {_service.TareasPorPrioridad(PrioridadTarea.Baja)}");

            Console.WriteLine("\n--- Por tipo ---");
            Console.WriteLine($"Simples: {_service.TareasSimples}");
            Console.WriteLine($"Recurrentes: {_service.TareasRecurrentes}");
            Console.WriteLine($"Urgentes: {_service.TareasUrgentes}");

            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        private void ListarTareasConIndices(System.Collections.Generic.List<Tarea> tareas)
        {
            Console.Clear();
            Console.WriteLine("=== SELECCIONE TAREA ===\n");
            for (int i = 0; i < tareas.Count; i++)
            {
                Console.Write($"[{i + 1}] ");
                tareas[i].Resumen();
            }
        }

        private bool ValidarTareasExistentes(System.Collections.Generic.List<Tarea> tareas)
        {
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas registradas. Presione Enter...");
                Console.ReadLine();
                return false;
            }
            return true;
        }
    }
}