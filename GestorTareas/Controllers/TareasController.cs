using System.Security.Claims;
using GestorTareas.Domain;
using GestorTareas.Services;
using GestorTareas.Services.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestorTareas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TareasController : ControllerBase
    {
        private readonly TareaService _servicio;

        public TareasController(TareaService servicio)
        {
            _servicio = servicio;
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);

            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();

            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            return Ok(tarea);
        }

        [HttpGet]
        public IActionResult ObtenerTodas([FromQuery] int pagina = 1, [FromQuery] int porPagina = 10)
        {
            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            var resultado = _servicio.ObtenerPaginado(pagina, porPagina, usuarioId, esAdmin);
            return Ok(resultado);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CrearTareaDto dto)
        {
            try
            {
                var usuarioId = ObtenerUsuarioId();
                Tarea tarea;

                switch (dto.Tipo)
                {
                    case 1:
                        tarea = new TareaSimple(dto.Titulo, dto.Descripcion, dto.FechaLimite, dto.Prioridad);
                        break;
                    case 2:
                        tarea = new TareaRecurrente(dto.Titulo, dto.Descripcion, dto.FechaLimite, dto.Prioridad, dto.IntervaloEnDias ?? 1);
                        break;
                    case 3:
                        tarea = new TareaUrgente(dto.Titulo, dto.Descripcion, dto.FechaLimite, dto.Prioridad, dto.Responsable);
                        break;
                    default:
                        return BadRequest("Tipo inválido");
                }

                _servicio.AgregarTarea(tarea, usuarioId);
                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Editar(Guid id, [FromBody] CrearTareaDto dto)
        {
            var tarea = _servicio.ObtenerPorId(id);
            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            tarea.Titulo = dto.Titulo;
            tarea.Descripcion = dto.Descripcion;
            tarea.FechaLimite = dto.FechaLimite;
            tarea.Prioridad = dto.Prioridad;

            if (tarea is TareaRecurrente recurrente && dto.IntervaloEnDias.HasValue)
                recurrente.IntervaloEnDias = dto.IntervaloEnDias.Value;

            if (tarea is TareaUrgente urgente && !string.IsNullOrWhiteSpace(dto.Responsable))
                urgente.Responsable = dto.Responsable;

            _servicio.GuardarCambios(tarea);
            return Ok(tarea);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Eliminar(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);
            if (tarea == null)
                return NotFound();

            _servicio.EliminarTarea(id);
            return Ok();
        }

        [HttpPost("{id}/cancelar")]
        public IActionResult Cancelar(Guid id, [FromBody] string? motivo)
        {
            var tarea = _servicio.ObtenerPorId(id);
            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            if (!tarea.Cancelar(motivo ?? "Cancelada por el usuario"))
                return BadRequest("No se pudo cancelar la tarea.");

            _servicio.GuardarCambios(tarea);
            return Ok(tarea);
        }

        [HttpGet("estado/{estado}")]
        public IActionResult ObtenerPorEstado(EstadoTarea estado)
        {
            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            var tareas = _servicio.ObtenerTodas()
                .Where(t => t.Estado == estado && (esAdmin || t.UsuarioId == usuarioId));

            return Ok(tareas);
        }

        [HttpPost("{id}/iniciar")]
        public IActionResult Iniciar(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);
            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            if (!tarea.Iniciar())
                return BadRequest("No se pudo iniciar la tarea. Solo se puede iniciar desde Pendiente.");

            _servicio.GuardarCambios(tarea);
            return Ok(tarea);
        }

        [HttpPost("{id}/completar")]
        public IActionResult Completar(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);
            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            if (!tarea.Completar())
                return BadRequest("No se pudo completar la tarea.");

            _servicio.GuardarCambios(tarea);
            return Ok(tarea);
        }

        [HttpPost("{id}/generar-siguiente")]
        public IActionResult GenerarSiguiente(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);

            if (tarea == null)
                return NotFound();

            var usuarioId = ObtenerUsuarioId();
            var esAdmin = EsAdmin();
            if (!_servicio.PuedeAdministrarTarea(tarea, usuarioId, esAdmin))
                return Forbid();

            if (tarea is not TareaRecurrente recurrente)
                return BadRequest("La tarea no es recurrente");

            _servicio.GenerarSiguienteRecurrente(recurrente);
            return Ok();
        }

        [HttpGet("estadisticas")]
        [Authorize(Roles = "admin")]
        public IActionResult Estadisticas()
        {
            return Ok(new
            {
                total = _servicio.TotalTareas,
                pendientes = _servicio.TareasPendientes,
                enProgreso = _servicio.TareasEnProgreso,
                completadas = _servicio.TareasCompletadas,
                canceladas = _servicio.TareasCanceladas,
                eliminadas = _servicio.TareasEliminadas,
                vencidas = _servicio.TareasVencidas,
                simples = _servicio.TareasSimples,
                recurrentes = _servicio.TareasRecurrentes,
                urgentes = _servicio.TareasUrgentes
            });
        }

        private Guid ObtenerUsuarioId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(claim!);
        }

        private bool EsAdmin()
        {
            return User.IsInRole("admin");
        }
    }
}
