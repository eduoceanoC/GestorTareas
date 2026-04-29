using Microsoft.AspNetCore.Mvc;
using GestorTareas.Services;
using GestorTareas.Domain;

namespace GestorTareas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TareasController : ControllerBase
    {
        private readonly TareaService _servicio;

        public TareasController(TareaService servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public IActionResult ObtenerTodas()
        {
            return Ok(_servicio.ObtenerTodas());
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);

            if (tarea == null)
                return NotFound();

            return Ok(tarea);
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CrearTareaDto dto)
        {
            try
            {
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

                _servicio.AgregarTarea(tarea);

                return Ok(tarea);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Eliminar(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);

            if (tarea == null)
                return NotFound();

            _servicio.EliminarTarea(id);

            return Ok();
        }

        [HttpGet("estado/{estado}")]
        public IActionResult ObtenerPorEstado(EstadoTarea estado)
        {
            return Ok(_servicio.ObtenerTodas()
                .Where(t => t.Estado == estado));
        }

        [HttpPost("{id}/generar-siguiente")]
        public IActionResult GenerarSiguiente(Guid id)
        {
            var tarea = _servicio.ObtenerPorId(id);

            if (tarea == null)
                return NotFound();

            if (tarea is not TareaRecurrente recurrente)
                return BadRequest("La tarea no es recurrente");

            _servicio.GenerarSiguienteRecurrente(recurrente);

            return Ok();
        }

        [HttpGet("estadisticas")]
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
    }
}