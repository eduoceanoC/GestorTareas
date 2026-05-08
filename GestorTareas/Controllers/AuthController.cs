using GestorTareas.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestorTareas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto dto)
        {
            var response = _authService.Login(dto.Nombre, dto.Password);

            if (response == null)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegistroRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            var response = _authService.Registrar(dto.Nombre, dto.Password);

            if (response == null)
            {
                return Conflict("El nombre de usuario ya está registrado.");
            }

            return Ok(response);
        }
    }
}