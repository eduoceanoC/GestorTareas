# MEMORIA DEL PROYECTO - GESTORTAREAS
## Documento de contexto persistente para IA

### Última actualización
14/05/2026 13:40

### ¿Qué es este archivo?
Este archivo contiene la memoria permanente de mi proyecto. Tú, como IA, DEBES leerlo antes de responder cualquier pregunta y actualizarlo después de cada cambio significativo en el código.

---

## ESTRUCTURA DEL PROYECTO
```
GestorTareas/
├── Controllers/
│   ├── AuthController.cs
│   └── TareasController.cs
├── Data/
│   ├── AppDbContext.cs
│   ├── Tarea.cs (clase abstracta base)
│   ├── TareaSimple.cs
│   ├── TareaRecurrente.cs
│   ├── TareaUrgente.cs
│   ├── Usuario.cs
│   ├── Estado.cs
│   └── Prioridad.cs
├── Domain/
│   ├── ITareaRepository.cs
│   └── EfTareaRepository.cs
├── Infrastructure/
│   └── Middleware/
│       └── ErrorHandlingMiddleware.cs
├── Services/
│   ├── TareaService.cs
│   ├── Dto/
│   │   ├── PaginadoResponseDto.cs
│   │   └── CrearTareaDto.cs
│   └── Auth/
│       ├── AuthService.cs
│       ├── AuthDtos.cs
│       └── JwtSettings.cs
├── UI/
│   └── ConsoleUI.cs
├── Migrations/
├── Program.cs
├── appsettings.json
├── GestorTareas.csproj
├── Properties/
│   └── launchSettings.json
└── globalrules.txt

GestorTareas.Frontend/
├── index.html
└── styles.css

TestProject1/
├── Tests.csproj
├── UnitTest1.cs
└── GestorTareasServiceTests.cs
```

---

## ENDPOINTS DEL BACKEND
- POST /api/auth/login - Inicio de sesión (público)
- POST /api/auth/register - Registro de usuario (público)
- GET /api/tareas - Listar tareas paginadas (?pagina=1&porPagina=10)
- GET /api/tareas/{id} - Obtener tarea por ID
- POST /api/tareas - Crear tarea
- PUT /api/tareas/{id} - Editar tarea
- DELETE /api/tareas/{id} - Eliminar tarea (solo admin)
- POST /api/tareas/{id}/iniciar - Iniciar tarea (Pendiente → EnProgreso)
- POST /api/tareas/{id}/completar - Completar tarea
- POST /api/tareas/{id}/cancelar - Cancelar tarea
- POST /api/tareas/{id}/generar-siguiente - Generar siguiente recurrente
- GET /api/tareas/estado/{estado} - Filtrar por estado
- GET /api/tareas/estadisticas - Estadísticas globales (solo admin)

---

## ESTRUCTURA DE BASE DE DATOS

### Usuarios
| Columna   | Tipo               | Restricciones          |
|-----------|--------------------|------------------------|
| Id        | UNIQUEIDENTIFIER   | PK, DEFAULT NEWID()    |
| Nombre    | NVARCHAR(100)      | NOT NULL, UNIQUE       |
| Password  | NVARCHAR(200)      | NOT NULL               |
| Rol       | NVARCHAR(20)       | NOT NULL ('admin'/'user') |

### Tareas (Tabla por jerarquía con discriminador TipoTarea)
| Columna           | Tipo               | Restricciones                          |
|-------------------|--------------------|----------------------------------------|
| Id                | UNIQUEIDENTIFIER   | PK, DEFAULT NEWID()                    |
| UsuarioId         | UNIQUEIDENTIFIER   | FK → Usuarios(Id), NOT NULL            |
| Titulo            | NVARCHAR(200)      | NOT NULL                               |
| Descripcion       | NVARCHAR(MAX)      | NULL                                   |
| FechaCreacion     | DATETIME2          | NOT NULL, DEFAULT GETDATE()            |
| FechaLimite       | DATETIME2          | NOT NULL                               |
| Prioridad         | TINYINT            | NOT NULL (0,1,2)                       |
| Estado            | TINYINT            | NOT NULL (0,1,2,3)                     |
| MotivoCancelacion | NVARCHAR(500)      | NULL                                   |
| TipoTarea         | TINYINT            | Discriminador (1=Simple, 2=Recurrente, 3=Urgente) |
| IntervaloEnDias   | INT                | NULL (solo Recurrente)                 |
| Responsable       | NVARCHAR(100)      | NULL (solo Urgente)                    |
| FechaLimiteHora   | DATETIME2          | NULL (solo Urgente)                    |

### Estadisticas
| Clave | Valor |
|-------|-------|
| TareasEliminadas | INT |

---

## FRONTEND - VENTANAS Y COMPONENTES
- win-inicio (pantalla de login/registro inicial)
- win-login (formulario de inicio de sesión)
- win-register (formulario de registro)
- win-menu (menú principal post-login)
- win-agregar (formulario para crear tareas)
- win-listar (lista de tareas del usuario actual)
- win-editar (editar datos o cambiar estado de tarea propia)
- win-cancelar (cancelar tarea - obsoleto, la opción se quitó del menú)
- win-stats (estadísticas personales del usuario)
- win-admin-todas (listado de todas las tareas del sistema con acciones)
- win-admin-gestion (gestión de tareas de un usuario específico)
- win-admin-stats (estadísticas globales del sistema)
- win-confirmacion (overlay flotante para confirmar eliminaciones)

---

## ESTADOS DE TAREA
0 = Pendiente
1 = EnProgreso
2 = Completada
3 = Cancelada

---

## TIPOS DE TAREA
1 = Simple
2 = Recurrente
3 = Urgente

---

## PRIORIDAD
0 = Baja
1 = Media
2 = Alta

---

## BUGS CONOCIDOS ACTUALMENTE
- La confirmación de eliminación mediante overlay flotante (win-confirmacion) usa una callback global (window.__eliminarCallback) que puede dar problemas si se disparan múltiples eventos de eliminación simultáneos. Mejor usar una variable de estado dentro del closure de la IIFE.
- El menú de "Editar/Cambiar estado" muestra opciones 1/2/0 pero luego redirige a otro menú con acciones Iniciar/Completar/Cancelar - hay navegación excesiva entre pantallas.
- Los endpoints de iniciar/completar existen en el backend pero no se usan en el flujo de admin-gestionar-usuario (menu interior), donde aún usa prompts y confirm() de navegador.
- La vista win-cancelar sigue existiendo en el HTML pero ya no es accesible desde el menú principal (se eliminó su entrada).

---

## CAMBIOS RECIENTES
- Añadidos endpoints POST /api/tareas/{id}/iniciar y POST /api/tareas/{id}/completar
- Configurado ReferenceHandler.IgnoreCycles para evitar error de serialización JSON por ciclo Tarea↔Usuario
- Eliminada opción "Cancelar" del menú principal (ahora solo dentro de Editar)
- Eliminada opción "Salir" del menú principal
- Renumeradas las opciones del menú para usuarios normales y admin
- Corregido listarMisTareas() y abrirEditar() para filtrar solo tareas del usuario actual
- Restaurados botones rápidos [COMPLETAR] y [ELIMINAR] en vista admin, ahora funcionales
- Reemplazado confirm() del navegador por overlay flotante de confirmación (win-confirmacion)
- Los botones de acción rápida ahora solo aparecen en vista de admin (conDetallesAdmin=true)
- Añadido indicador visual "✓ COMPLETADA" en verde para tareas completadas

---

## HISTORIAL DE CONVERSACIONES

### 14/05/2026
- Tarea inicial: Corrección de error de serialización JSON por ciclo de referencias Tarea↔Usuario (ReferenceHandler.IgnoreCycles)
- Corrección de frontend para manejar respuesta paginada de API (data.datos)
- Reorganización del menú: eliminadas opciones "Salir" y "Cancelar" redundante, renumeración
- Corrección de filtrado: listarMisTareas() y abrirEditar() ahora filtran por usuario actual
- Restaurados y corregidos botones rápidos [COMPLETAR] y [ELIMINAR] en vista admin
- Reemplazado confirm() del navegador por overlay flotante de confirmación (win-confirmacion)
- Añadidos endpoints de backend: POST /api/tareas/{id}/iniciar y POST /api/tareas/{id}/completar
- Creación de MEMORIA.md como documento de contexto persistente para IA

---

## TAREAS PENDIENTES
- Reemplazar prompt() del navegador por inputs inline estilo terminal en admin-gestionar-usuario (editar datos, cancelar)
- Reemplazar confirm() del navegador por overlay flotante en admin-gestionar-usuario (eliminar)
- Refactorizar window.__eliminarCallback para que sea una variable de estado local dentro de la IIFE
- Simplificar el flujo de Editar/Cambiar estado para que sea menos anidado
- Eliminar la vista win-cancelar del HTML si ya no se usa