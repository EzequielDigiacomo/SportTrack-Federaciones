using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.DTOs.Usuario;
using SportTrack_v1.Entidades.DTOs.Participante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using SportTrack_v1.Controladores.Federaciones;
using Microsoft.AspNetCore.Mvc;

namespace SIGDEF.API.Services
{
    public class UsuarioServices : IUsuarioServices
    {
        private readonly SportTrackDbContext _context;
        private readonly ITenantProvider _tenantProvider;

        public UsuarioServices(SportTrackDbContext context, ITenantProvider tenantProvider)
        {
            _context = context;
            _tenantProvider = tenantProvider;
        }

        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios()
        {
            try
            {
                var query = _context.Usuarios.AsQueryable();
                var fedId = _tenantProvider.GetFederacionId();
                if (fedId.HasValue)
                {
                    query = query.Where(u => u.IdFederacion == fedId.Value);
                }
                var clubId = _tenantProvider.GetClubId();
                if (clubId.HasValue)
                {
                    query = query.Where(u => u.IdClub == clubId.Value);
                }

                var usuarios = await query
                    .Include(u => u.Participante)
                    .Select(u => new UsuarioDto
                    {
                        IdUsuario = u.IdUsuario,
                        ParticipanteId = u.ParticipanteId,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        NombrePersona = u.Participante.Nombre + " " + u.Participante.Apellido,
                        Email = u.Participante.Email,
                        Rol = u.Rol,
                        IdFederacion = u.IdFederacion,
                        IdClub = u.IdClub
                    })
                    .ToListAsync();

                return new OkObjectResult(usuarios);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<UsuarioDetailDto>> GetUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Participante)
                    .Where(u => u.ParticipanteId == id)
                    .Select(u => new UsuarioDetailDto
                    {
                        ParticipanteId = u.ParticipanteId,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        Participante = new PersonaDto
                        {
                            ParticipanteId = u.Participante.ParticipanteId,
                            Nombre = u.Participante.Nombre,
                            Apellido = u.Participante.Apellido,
                            Documento = u.Participante.Dni,
                            FechaNacimiento = u.Participante.FechaNacimiento,
                            Email = u.Participante.Email,
                            Telefono = u.Participante.Telefono,
                            Direccion = u.Participante.Direccion
                        }
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(usuario);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<UsuarioDto>> GetUsuarioPorUsername(string username)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Participante)
                    .Where(u => u.Username == username)
                    .Select(u => new UsuarioDto
                    {
                        ParticipanteId = u.ParticipanteId,
                        Username = u.Username,
                        EstaActivo = u.EstaActivo,
                        FechaCreacion = u.FechaCreacion,
                        UltimoAcceso = u.UltimoAcceso,
                        NombrePersona = u.Participante.Nombre + " " + u.Participante.Apellido,
                        Email = u.Participante.Email,
                        Rol = "Usuario"
                    })
                    .FirstOrDefaultAsync();

                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                return new OkObjectResult(usuario);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<UsuarioDto>> PostUsuario(UsuarioCreateDto usuarioCreateDto)
        {
            try
            {
                var personaExists = await _context.Participantes.AnyAsync(p => p.ParticipanteId == usuarioCreateDto.ParticipanteId);
                if (!personaExists)
                {
                    return new BadRequestResult();
                }

                var usuarioExists = await _context.Usuarios.AnyAsync(u => u.ParticipanteId == usuarioCreateDto.ParticipanteId);
                if (usuarioExists)
                {
                    return new BadRequestResult();
                }

                var usernameExists = await _context.Usuarios.AnyAsync(u => u.Username == usuarioCreateDto.Username);
                if (usernameExists)
                {
                    return new BadRequestResult();
                }

                var passwordHash = HashPassword(usuarioCreateDto.Password);

                var fedId = _tenantProvider.GetFederacionId() ?? usuarioCreateDto.IdFederacion;
                var clubId = _tenantProvider.GetClubId() ?? usuarioCreateDto.IdClub;

                var usuario = new Usuario
                {
                    ParticipanteId = usuarioCreateDto.ParticipanteId,
                    Username = usuarioCreateDto.Username,
                    PasswordHash = passwordHash,
                    EstaActivo = usuarioCreateDto.EstaActivo,
                    FechaCreacion = DateTime.UtcNow,
                    UltimoAcceso = DateTime.UtcNow,
                    IdFederacion = fedId,
                    IdClub = clubId > 0 ? clubId : null,
                    Rol = usuarioCreateDto.Rol ?? "Usuario"
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                await _context.Entry(usuario)
                    .Reference(u => u.Participante)
                    .LoadAsync();

                var usuarioDto = new UsuarioDto
                {
                    ParticipanteId = usuario.ParticipanteId,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    NombrePersona = usuario.Participante.Nombre + " " + usuario.Participante.Apellido,
                    Email = usuario.Participante.Email,
                    Rol = "Usuario"
                };

                var result = new ObjectResult(usuarioDto)
                {
                    StatusCode = 201
                };
                return result;
            }
            catch (DbUpdateException)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<UsuarioDto>> Login(UsuarioLoginDto loginDto)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Participante)
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.EstaActivo);

                if (usuario == null)
                {
                    return new BadRequestResult();
                }

                if (!VerifyPassword(loginDto.Password, usuario.PasswordHash))
                {
                    return new BadRequestResult();
                }

                usuario.UltimoAcceso = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var usuarioDto = new UsuarioDto
                {
                    ParticipanteId = usuario.ParticipanteId,
                    Username = usuario.Username,
                    EstaActivo = usuario.EstaActivo,
                    FechaCreacion = usuario.FechaCreacion,
                    UltimoAcceso = usuario.UltimoAcceso,
                    NombrePersona = usuario.Participante.Nombre + " " + usuario.Participante.Apellido,
                    Email = usuario.Participante.Email,
                    Rol = "Usuario"
                };

                return new OkObjectResult(usuarioDto);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> PutUsuario(int id, UsuarioUpdateDto usuarioUpdateDto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                if (!string.IsNullOrEmpty(usuarioUpdateDto.Username) && usuarioUpdateDto.Username != usuario.Username)
                {
                    var usernameExists = await _context.Usuarios.AnyAsync(u => u.Username == usuarioUpdateDto.Username && u.ParticipanteId != id);
                    if (usernameExists)
                    {
                        return new BadRequestResult();
                    }
                    usuario.Username = usuarioUpdateDto.Username;
                }

                if (usuarioUpdateDto.EstaActivo.HasValue)
                {
                    usuario.EstaActivo = usuarioUpdateDto.EstaActivo.Value;
                }

                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await UsuarioExistsAsync(id))
                {
                    return new NotFoundResult();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> ChangePassword(int id, UsuarioChangePasswordDto changePasswordDto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                if (!VerifyPassword(changePasswordDto.CurrentPassword, usuario.PasswordHash))
                {
                    return new BadRequestResult();
                }

                usuario.PasswordHash = HashPassword(changePasswordDto.NewPassword);
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return new NoContentResult();
            }
            catch (DbUpdateException)
            {
                return new StatusCodeResult(500);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        public async Task<ActionResult<string>> ResetPassword(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                {
                    return new NotFoundResult();
                }

                // Generar clave aleatoria (8 caracteres alfanuméricos)
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Evitamos O, I, 0, 1 por confusión
                var random = new Random();
                var newPassword = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                // Hashear y actualizar
                usuario.PasswordHash = HashPassword(newPassword);
                await _context.SaveChangesAsync();

                return new OkObjectResult(newPassword);
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        private async Task<bool> UsuarioExistsAsync(int id)
        {
            return await _context.Usuarios.AnyAsync(e => e.ParticipanteId == id);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var hash = HashPassword(password);
            return hash == storedHash;
        }
    }
}
