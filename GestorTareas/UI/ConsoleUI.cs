using System;
using System.Linq;
using GestorTareas.Data;
using GestorTareas.Domain;
using GestorTareas.Services;
using GestorTareas.Services.Auth;

namespace GestorTareas.UI
{
    public class ConsoleUI
    {
        private readonly TareaService _service;
        private readonly AuthService _authService;
        private Usuario? _usuarioActual;

        public ConsoleUI(TareaService service, AuthService authService)
        {
            _service = service;
            _authService = authService;
        }

        public void Ejecutar()
        {
            while (true)
            {
                if (!MenuInicio())
                {
                    return;
                }

                bool salir = false;
                while (!salir)
                {
                    MostrarMenuPrincipal();
                    string opcion = Console.ReadLine() ?? string.Empty;

                    switch (opcion)
                    {
                        case "1": AgregarTarea(); break;
                        case "2": ListarMisTareas(); break;
                        case "3": EditarTareaUsuarioActual(); break;
                        case "4": CancelarMiTarea(); break;
                        case "5": MostrarEstadisticas(); break;
                        case "6" when EsAdmin(): VerTodasLasTareas(); break;
                        case "7" when EsAdmin(): AdministrarTareasDeUsuario(); break;
                        case "0":
                            Console.WriteLine("¡Hasta luego!");
                            return;
                        default:
                            if (opcion == (EsAdmin() ? "9" : "7"))
                            {
                                _usuarioActual = null;
                                salir = true;
                            }
                            else
                            {
                                Console.WriteLine("Opción no válida. Presione Enter para continuar...");
                                Console.ReadLine();
                            }
                            break;
                    }
                }
            }
        }

        private bool MenuInicio()
        {
            while (true)
            {
                LimpiarPantalla();
                Console.WriteLine("=== GESTOR DE TAREAS ===");
                Console.WriteLine("1. Iniciar sesión");
                Console.WriteLine("2. Crear usuario nuevo");
                Console.WriteLine("0. Salir");
                Console.Write("\nSeleccione una opción: ");
                string opcion = Console.ReadLine() ?? string.Empty;

                switch (opcion)
                {
                    case "1":
                        if (Login()) return true;
                        break;
                    case "2":
                        CrearUsuario();
                        break;
                    case "0":
                        Console.WriteLine("¡Hasta luego!");
                        return false;
                    default:
                        Console.WriteLine("Opción no válida. Presione Enter...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        private bool Login()
        {
            LimpiarPantalla();
            Console.WriteLine("=== INICIAR SESIÓN ===");
            Console.Write("Usuario: ");
            var nombre = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(nombre)) return false;

            Console.Write("Contraseña: ");
            var password = Console.ReadLine() ?? string.Empty;

            var usuario = _authService.ValidarCredencialesPorNombre(nombre, password);
            if (usuario == null)
            {
                Console.WriteLine("\n✗ Usuario o contraseña incorrectos. Presione Enter...");
                Console.ReadLine();
                return false;
            }

            _usuarioActual = usuario;
            return true;
        }

        private void CrearUsuario()
        {
            LimpiarPantalla();
            Console.WriteLine("=== CREAR NUEVO USUARIO ===");
            Console.Write("Nombre de usuario: ");
            var nombre = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                Console.WriteLine("Nombre no válido. Presione Enter...");
                Console.ReadLine();
                return;
            }

            Console.Write("Contraseña: ");
            var password = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Contraseña no válida. Presione Enter...");
                Console.ReadLine();
                return;
            }

            var resultado = _authService.RegistrarPorNombre(nombre, password);
            if (resultado == null)
            {
                Console.WriteLine("\n✗ El nombre de usuario ya existe. Presione Enter...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"\n✓ Usuario '{nombre}' creado correctamente. Presione Enter...");
            Console.ReadLine();
        }

        private void MostrarMenuPrincipal()
        {
            LimpiarPantalla();
            Console.WriteLine($"=== GESTOR DE TAREAS | Usuario: {_usuarioActual!.Nombre} ({_usuarioActual.Rol}) ===");
            Console.WriteLine("1. Agregar nueva tarea");
            Console.WriteLine("2. Listar mis tareas");
            Console.WriteLine("3. Editar una de mis tareas");
            Console.WriteLine("4. Cancelar una de mis tareas");
            Console.WriteLine("5. Ver mis estadísticas");
            if (EsAdmin())
            {
                Console.WriteLine("6. [Admin] Ver todas las tareas del sistema");
                Console.WriteLine("7. [Admin] Gestionar tareas de un usuario");
                Console.WriteLine("8. [Admin] Estadísticas globales");
                Console.WriteLine("9. Cerrar sesión");
            }
            else
            {
                Console.WriteLine("6. Cerrar sesión");
            }
            Console.WriteLine("0. Salir");
            Console.Write("\nSeleccione una opción: ");
        }

        private void AgregarTarea()
        {
            LimpiarPantalla();
            Console.WriteLine("=== AGREGAR NUEVA TAREA ===");
            Console.WriteLine("Tipos de tarea:");
            Console.WriteLine("1. Tarea Simple");
            Console.WriteLine("2. Tarea Recurrente");
            Console.WriteLine("3. Tarea Urgente");
            Console.Write("Seleccione tipo: ");
            string tipo = Console.ReadLine() ?? string.Empty;

            Console.Write("Título: ");
            string titulo = Console.ReadLine() ?? string.Empty;
            Console.Write("Descripción: ");
            string descripcion = Console.ReadLine() ?? string.Empty;
            Console.Write("Prioridad (1=Baja, 2=Media, 3=Alta): ");
            string prioridadInput = Console.ReadLine() ?? string.Empty;

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
                        _service.AgregarTarea(new TareaSimple(titulo, descripcion ?? string.Empty, fechaLimite, prioridad), _usuarioActual!.Id);
                        Console.WriteLine("\n✓ Tarea simple agregada exitosamente!");
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
                        _service.AgregarTarea(new TareaRecurrente(titulo, descripcion ?? string.Empty, fechaLimite, prioridad, intervalo), _usuarioActual!.Id);
                        Console.WriteLine("\n✓ Tarea recurrente agregada exitosamente!");
                        break;
                    case "3":
                        Console.Write("Fecha límite (dd/MM/yyyy HH:mm): ");
                        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out fechaLimite))
                        {
                            Console.WriteLine("Fecha inválida. Operación cancelada.");
                            break;
                        }
                        Console.Write("Responsable: ");
                        string responsable = Console.ReadLine() ?? string.Empty;
                        _service.AgregarTarea(new TareaUrgente(titulo, descripcion ?? string.Empty, fechaLimite, prioridad, responsable), _usuarioActual!.Id);
                        Console.WriteLine("\n✓ Tarea urgente agregada exitosamente!");
                        break;
                    default:
                        Console.WriteLine("Tipo no válido");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n✗ Error al crear tarea: {ex.Message}");
            }

            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        private void ListarMisTareas()
        {
            LimpiarPantalla();
            Console.WriteLine("=== MIS TAREAS ===\n");

            var tareas = _service.ObtenerPorUsuario(_usuarioActual!.Id);
            MostrarListado(tareas, conDetallesAdmin: false);
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        private void EditarTareaUsuarioActual()
        {
            var tareas = _service.ObtenerPorUsuario(_usuarioActual!.Id);
            if (!ValidarTareasExistentes(tareas)) return;

            ListarTareasConIndices(tareas, "=== EDITAR MIS TAREAS ===");
            Console.Write("\nSeleccione el número de tarea a editar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            EditarDatosTarea(tareas[indice - 1]);
        }

        private void CancelarMiTarea()
        {
            var tareas = _service.ObtenerPorUsuario(_usuarioActual!.Id)
                .Where(t => t.Estado != EstadoTarea.Cancelada)
                .ToList();

            if (!ValidarTareasExistentes(tareas)) return;

            ListarTareasConIndices(tareas, "=== CANCELAR MIS TAREAS ===");
            Console.Write("\nSeleccione el número de tarea a cancelar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            Console.Write("Motivo de cancelación: ");
            var motivo = Console.ReadLine();
            var exito = tareas[indice - 1].Cancelar(motivo ?? "Cancelada por el usuario");
            if (exito)
            {
                _service.GuardarCambios(tareas[indice - 1]);
                Console.WriteLine("✓ Tarea cancelada correctamente");
            }
            else
            {
                Console.WriteLine("✗ No se pudo cancelar la tarea");
            }
            Console.ReadLine();
        }

        // ============================================================
        //  OPCIONES DE ADMIN
        // ============================================================

        private void VerTodasLasTareas()
        {
            LimpiarPantalla();
            Console.WriteLine("=== TODAS LAS TAREAS DEL SISTEMA ===\n");

            var tareas = _service.ObtenerTodas();
            MostrarListado(tareas, conDetallesAdmin: true);
            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        private void AdministrarTareasDeUsuario()
        {
            var usuarioId = PedirUsuarioId();
            if (usuarioId == null) return;

            var tareas = _service.ObtenerPorUsuario(usuarioId.Value);
            LimpiarPantalla();
            Console.WriteLine($"=== TAREAS DEL USUARIO {usuarioId} ===\n");

            if (tareas.Count == 0)
            {
                Console.WriteLine("Este usuario no tiene tareas.");
                Console.WriteLine("\nPresione Enter para continuar...");
                Console.ReadLine();
                return;
            }

            MostrarListado(tareas, conDetallesAdmin: true);

            Console.WriteLine("\n--- ACCIONES ---");
            Console.WriteLine("1. Editar una tarea");
            Console.WriteLine("2. Cambiar estado de una tarea");
            Console.WriteLine("3. Eliminar una tarea (borrado completo de BD)");
            Console.WriteLine("0. Volver al menú principal");
            Console.Write("\nSeleccione: ");

            string opcion = Console.ReadLine() ?? string.Empty;
            switch (opcion)
            {
                case "1":
                    EditarTareaAdmin(tareas);
                    break;
                case "2":
                    CambiarEstadoTareaAdmin(tareas);
                    break;
                case "3":
                    EliminarTareaAdmin(tareas);
                    break;
                default:
                    return;
            }
        }

        private void EditarTareaAdmin(System.Collections.Generic.List<Tarea> tareas)
        {
            ListarTareasConIndices(tareas, "=== EDITAR TAREA (ADMIN) ===");
            Console.Write("\nSeleccione el número de tarea a editar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            EditarDatosTarea(tareas[indice - 1]);
        }

        private void CambiarEstadoTareaAdmin(System.Collections.Generic.List<Tarea> tareas)
        {
            ListarTareasConIndices(tareas, "=== CAMBIAR ESTADO (ADMIN) ===");
            Console.Write("\nSeleccione la tarea: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            CambiarEstadoInteractivo(tareas[indice - 1]);
        }

        private void EliminarTareaAdmin(System.Collections.Generic.List<Tarea> tareas)
        {
            ListarTareasConIndices(tareas, "=== ELIMINAR TAREA (ADMIN) ===");
            Console.Write("\nSeleccione la tarea a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 1 || indice > tareas.Count)
            {
                Console.WriteLine("Selección no válida");
                Console.ReadLine();
                return;
            }

            _service.EliminarTarea(tareas[indice - 1].Id);
            Console.WriteLine("✓ Tarea eliminada permanentemente de la BD");
            Console.ReadLine();
        }

        private void MostrarEstadisticas()
        {
            LimpiarPantalla();
            Console.WriteLine("=== MIS ESTADÍSTICAS ===\n");

            var misTareas = _service.ObtenerPorUsuario(_usuarioActual!.Id);
            var total = misTareas.Count;
            var pendientes = misTareas.Count(t => t.Estado == EstadoTarea.Pendiente);
            var enProgreso = misTareas.Count(t => t.Estado == EstadoTarea.EnProgreso);
            var completadas = misTareas.Count(t => t.Estado == EstadoTarea.Completada);
            var canceladas = misTareas.Count(t => t.Estado == EstadoTarea.Cancelada);
            var vencidas = misTareas.Count(t => t.EstaVencida);
            var alta = misTareas.Count(t => t.Prioridad == PrioridadTarea.Alta);
            var media = misTareas.Count(t => t.Prioridad == PrioridadTarea.Media);
            var baja = misTareas.Count(t => t.Prioridad == PrioridadTarea.Baja);

            Console.WriteLine($"Total tareas: {total}");
            Console.WriteLine($"├─ Pendientes: {pendientes}");
            Console.WriteLine($"├─ En progreso: {enProgreso}");
            Console.WriteLine($"├─ Completadas: {completadas}");
            Console.WriteLine($"└─ Canceladas: {canceladas}");
            Console.WriteLine($"\nTareas vencidas: {vencidas}");
            Console.WriteLine("\n--- Por prioridad ---");
            Console.WriteLine($"Alta: {alta}");
            Console.WriteLine($"Media: {media}");
            Console.WriteLine($"Baja: {baja}");

            Console.WriteLine("\nPresione Enter para continuar...");
            Console.ReadLine();
        }

        // ============================================================
        //  MÉTODOS COMPARTIDOS
        // ============================================================

        private Guid? PedirUsuarioId()
        {
            LimpiarPantalla();
            Console.WriteLine("=== BUSCAR USUARIO ===");
            Console.WriteLine("Introduce el Guid del usuario para gestionar sus tareas.");
            Console.WriteLine("Admin: 11111111-1111-1111-1111-111111111111");
            Console.Write("UsuarioId: ");
            if (!Guid.TryParse(Console.ReadLine(), out var usuarioId))
            {
                Console.WriteLine("Guid inválido.");
                Console.ReadLine();
                return null;
            }
            return usuarioId;
        }

        private void EditarDatosTarea(Tarea tarea)
        {
            LimpiarPantalla();
            Console.WriteLine($"=== EDITANDO TAREA: {tarea.Titulo} ===\n");

            Console.Write($"Nuevo título (actual: {tarea.Titulo}): ");
            string nuevoTitulo = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nuevoTitulo)) tarea.Titulo = nuevoTitulo;

            Console.Write($"Nueva descripción (actual: {tarea.Descripcion}): ");
            string nuevaDesc = Console.ReadLine() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(nuevaDesc)) tarea.Descripcion = nuevaDesc;

            Console.Write($"Nueva prioridad (1=Baja,2=Media,3=Alta) (actual: {tarea.Prioridad}): ");
            string nuevaPrioridad = Console.ReadLine() ?? string.Empty;
            if (int.TryParse(nuevaPrioridad, out int prioridadVal) && prioridadVal >= 1 && prioridadVal <= 3)
                tarea.Prioridad = (PrioridadTarea)(prioridadVal - 1);

            if (tarea is TareaRecurrente recurrente)
            {
                Console.Write($"Nuevo intervalo en días (actual: {recurrente.IntervaloEnDias}): ");
                string nuevoIntervalo = Console.ReadLine() ?? string.Empty;
                if (int.TryParse(nuevoIntervalo, out int intervaloVal) && intervaloVal > 0)
                    recurrente.IntervaloEnDias = intervaloVal;
            }
            else if (tarea is TareaUrgente urgente)
            {
                Console.Write($"Nuevo responsable (actual: {urgente.Responsable}): ");
                string nuevoResp = Console.ReadLine() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(nuevoResp)) urgente.Responsable = nuevoResp;
            }

            _service.GuardarCambios(tarea);
            Console.WriteLine("\n✓ Tarea actualizada exitosamente!");
            Console.ReadLine();
        }

        private void CambiarEstadoInteractivo(Tarea tarea)
        {
            LimpiarPantalla();
            Console.WriteLine($"=== CAMBIAR ESTADO: {tarea.Titulo} ===");
            Console.WriteLine($"Estado actual: {tarea.Estado}");
            Console.WriteLine("\nNuevo estado:");
            Console.WriteLine("1. Iniciar (Pendiente → EnProgreso)");
            Console.WriteLine("2. Completar");
            Console.WriteLine("3. Cancelar");
            Console.Write("Seleccione: ");

            string opcion = Console.ReadLine() ?? string.Empty;
            bool exito = false;

            switch (opcion)
            {
                case "1": exito = tarea.Iniciar(); break;
                case "2": exito = tarea.Completar(); break;
                case "3":
                    Console.Write("Motivo de cancelación: ");
                    string motivo = Console.ReadLine() ?? string.Empty;
                    exito = tarea.Cancelar(motivo);
                    break;
            }

            if (exito)
                _service.GuardarCambios(tarea);

            Console.WriteLine(exito ? "\n✓ Estado actualizado!" : "\n✗ No se pudo cambiar el estado");
            Console.ReadLine();
        }

        private void ListarTareasConIndices(System.Collections.Generic.List<Tarea> tareas, string titulo, bool conDetallesAdmin = false)
        {
            LimpiarPantalla();
            Console.WriteLine($"{titulo}\n");
            for (int i = 0; i < tareas.Count; i++)
            {
                Console.WriteLine($"[{i + 1}]");
                MostrarTarea(tareas[i], conDetallesAdmin);
            }
        }

        private void MostrarListado(System.Collections.Generic.List<Tarea> tareas, bool conDetallesAdmin = false)
        {
            if (tareas.Count == 0)
            {
                Console.WriteLine("No hay tareas para mostrar.");
                return;
            }

            foreach (var t in tareas)
            {
                MostrarTarea(t, conDetallesAdmin);
            }
        }

        private void MostrarTarea(Tarea tarea, bool conDetallesAdmin = false)
        {
            // Vista para usuario normal (sin IDs internos)
            Console.WriteLine($"   [{tarea.Estado}] {tarea.Titulo}");
            Console.WriteLine($"   Descripción: {tarea.Descripcion}");
            Console.WriteLine($"   Fecha límite: {tarea.FechaLimite:dd/MM/yyyy}");
            Console.WriteLine($"   Prioridad: {tarea.Prioridad}");
            Console.WriteLine($"   Creada: {tarea.FechaCreacion:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"   Tipo: {ObtenerTipoTarea(tarea)}");

            if (tarea is TareaRecurrente rec)
                Console.WriteLine($"   Intervalo: cada {rec.IntervaloEnDias} días");
            if (tarea is TareaUrgente urg)
                Console.WriteLine($"   Responsable: {urg.Responsable}");
            if (tarea.Estado == EstadoTarea.Cancelada && !string.IsNullOrEmpty(tarea.MotivoCancelacion))
                Console.WriteLine($"   Motivo cancelación: {tarea.MotivoCancelacion}");
            if (tarea.EstaVencida)
                Console.WriteLine($"   ⚠ VENCIDA");

            // Solo admin ve detalles internos
            if (conDetallesAdmin)
            {
                Console.WriteLine($"   ID: {tarea.Id}");
                Console.WriteLine($"   UsuarioId: {tarea.UsuarioId}");
                Console.WriteLine($"   Usuario: {tarea.Usuario?.Nombre ?? "N/D"}");
            }

            Console.WriteLine();
        }

        private static string ObtenerTipoTarea(Tarea tarea)
        {
            return tarea switch
            {
                TareaSimple => "Simple",
                TareaRecurrente => "Recurrente",
                TareaUrgente => "Urgente",
                _ => "Desconocido"
            };
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

        private bool EsAdmin() => _usuarioActual?.Rol == "admin";

        private static void LimpiarPantalla()
        {
            try { Console.Clear(); } catch { }
        }
    }
}