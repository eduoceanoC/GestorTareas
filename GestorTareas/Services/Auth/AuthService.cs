using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GestorTareas.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GestorTareas.Services.Auth
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public AuthService(AppDbContext context, IOptions<JwtSettings> jwtOptions)
        {
            _context = context;
            _jwtSettings = jwtOptions.Value;
        }

        public LoginResponseDto? Login(string email, string password)
        {
            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                return null;

            return GenerarToken(usuario);
        }

        public LoginResponseDto? Registrar(string nombre, string email, string password)
        {
            if (_context.Usuarios.Any(u => u.Email == email))
                return null;

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Rol = "user"
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return GenerarToken(usuario);
        }

        public Usuario? ValidarCredenciales(string email, string password)
        {
            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == email);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                return null;

            return usuario;
        }

        public Usuario? ValidarCredencialesPorNombre(string nombre, string password)
        {
            var usuario = _context.Usuarios
                .AsNoTracking()
                .FirstOrDefault(u => u.Nombre == nombre);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.Password))
                return null;

            return usuario;
        }

        public Usuario? RegistrarPorNombre(string nombre, string password)
        {
            if (_context.Usuarios.Any(u => u.Nombre == nombre))
                return null;

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nombre = nombre,
                Email = $"{nombre}@gestortareas.local",
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Rol = "user"
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return usuario;
        }

        private LoginResponseDto GenerarToken(Usuario usuario)
        {
            var expiraEn = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email),
                new(JwtRegisteredClaimNames.UniqueName, usuario.Nombre),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(ClaimTypes.Name, usuario.Nombre),
                new(ClaimTypes.Email, usuario.Email),
                new(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiraEn,
                signingCredentials: creds);

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiraEn = expiraEn,
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }
    }
}