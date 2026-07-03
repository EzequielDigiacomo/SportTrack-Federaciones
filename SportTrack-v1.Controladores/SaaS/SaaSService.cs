using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SportTrack.AccessDatos;
using SportTrack_v1.Controladores.SaaS.Dtos;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;
using SportTrack_v1.Controladores.Audit;

namespace SportTrack_v1.Controladores.SaaS
{
    public class SaaSService : ISaaSService
    {
        private readonly SportTrackDbContext _context;
        private readonly IAuditService _auditService;

        public SaaSService(SportTrackDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        public async Task<IEnumerable<PlanSaaSDto>> GetPlanesAsync()
        {
            var planes = await _context.PlanesSaaS.ToListAsync();
            return planes.Select(p => new PlanSaaSDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                MaxAtletas = p.MaxAtletas,
                MaxTorneosActivos = p.MaxTorneosActivos,
                ResultadosTiempoReal = p.ResultadosTiempoReal,
                ExportacionExcel = p.ExportacionExcel,
                SoportePrioritario = p.SoportePrioritario
            });
        }

        public async Task<PlanSaaSDto> GetPlanByIdAsync(int id)
        {
            var p = await _context.PlanesSaaS.FindAsync(id);
            if (p == null) return null;

            return new PlanSaaSDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Precio = p.Precio,
                MaxAtletas = p.MaxAtletas,
                MaxTorneosActivos = p.MaxTorneosActivos,
                ResultadosTiempoReal = p.ResultadosTiempoReal,
                ExportacionExcel = p.ExportacionExcel,
                SoportePrioritario = p.SoportePrioritario
            };
        }

        public async Task AsignarPlanAClubAsync(int federacionId, int planId)
        {
            var fed = await _context.Federaciones.FindAsync(federacionId);
            if (fed != null)
            {
                var oldPlanId = fed.PlanSaaSId;
                fed.PlanSaaSId = planId;
                await _context.SaveChangesAsync();

                if (oldPlanId != planId)
                {
                    var plan = await _context.PlanesSaaS.FindAsync(planId);
                    string planNombre = plan?.Nombre ?? $"Plan ID {planId}";
                    await _auditService.RegistrarAccionAsync(
                        "ASSIGN_PLAN",
                        $"Asignado Plan '{planNombre}' a la federación '{fed.Nombre}'.",
                        modulo: "SaaS"
                    );
                }
            }
        }

        public async Task<IEnumerable<ClubSaaSStatusDto>> GetClubesStatusAsync()
        {
            // Plan basico por defecto si no tiene plan (ID 1)
            var planBasico = await _context.PlanesSaaS.FirstOrDefaultAsync(p => p.Id == 1);

            var federaciones = await _context.Federaciones
                .Include(f => f.PlanSaaS)
                .Include(f => f.Usuarios)
                .Include(f => f.Clubes)
                    .ThenInclude(c => c.Participantes)
                .Include(f => f.Clubes)
                    .ThenInclude(c => c.Usuarios)
                .ToListAsync();

            // Buscamos todos los torneos activos para agruparlos por federación madre
            var eventosActivos = await _context.Eventos
                .Where(e => (e.Estado == Entidades.Enums.EstadoEventoEnum.Programada || e.Estado == Entidades.Enums.EstadoEventoEnum.EnCurso) && e.FederacionId.HasValue)
                .Select(e => new { e.FederacionId, e.Id, e.Nombre, e.Fecha, Estado = e.Estado.ToString() })
                .ToListAsync();

            return federaciones.Select(c => 
            {
                var planActivo = c.PlanSaaS ?? planBasico;
                var maxAtletas = planActivo?.MaxAtletas ?? 500;
                var maxTorneos = planActivo?.MaxTorneosActivos ?? 1;

                var atletasRegistrados = c.Clubes.Sum(a => a.Participantes.Count);
                var usuariosCount = c.Usuarios.Count + c.Clubes.Sum(a => a.Usuarios.Count);
                
                var torneosDetalle = eventosActivos
                    .Where(e => e.FederacionId == c.Id)
                    .Select(e => new TorneoSaaSDetailDto { Id = e.Id, Nombre = e.Nombre, Fecha = e.Fecha, Estado = e.Estado })
                    .ToList();
                
                var torneosActivosCount = torneosDetalle.Count;

                var alDia = true;
                if (maxAtletas != -1 && atletasRegistrados > maxAtletas) alDia = false;
                if (maxTorneos != -1 && torneosActivosCount > maxTorneos) alDia = false;
                if (c.FechaVencimientoPlan.HasValue && c.FechaVencimientoPlan.Value.Date < DateTime.UtcNow.Date) alDia = false;
                if (c.BloqueadaPorFaltaDePago) alDia = false;

                return new ClubSaaSStatusDto
                {
                    ClubId = c.Id,
                    ClubNombre = c.Nombre,
                    Sigla = c.Sigla,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion,
                    Ubicacion = "",
                    PlanSaaSId = planActivo?.Id,
                    PlanNombre = planActivo?.Nombre ?? "Desconocido",
                    MaxAtletas = maxAtletas,
                    AtletasRegistrados = atletasRegistrados,
                    ClubesAfiliadosCount = c.Clubes.Count,
                    UsuariosCount = usuariosCount,
                    MaxTorneos = maxTorneos,
                    TorneosActivosCount = torneosActivosCount,
                    TorneosActivos = torneosDetalle,
                    PlanAlDia = alDia,
                    Activo = c.Activo,
                    FrecuenciaPago = "",
                    FechaAltaPlan = c.FechaAltaPlan,
                    FechaVencimientoPlan = c.FechaVencimientoPlan,
                    BloqueadoPorFaltaDePago = c.BloqueadaPorFaltaDePago
                };
            });
        }

        public async Task ToggleClubActivoAsync(int federacionId)
        {
            var fed = await _context.Federaciones.FindAsync(federacionId);
            if (fed != null)
            {
                fed.Activo = !fed.Activo;
                await _context.SaveChangesAsync();

                string status = fed.Activo ? "habilitado" : "suspendido";
                string accion = fed.Activo ? "ACTIVATE_FEDERATION" : "SUSPEND_FEDERATION";
                await _auditService.RegistrarAccionAsync(
                    accion,
                    $"Acceso a la federación '{fed.Nombre}' {status} manualmente.",
                    modulo: "SaaS"
                );
            }
        }

        public async Task<int> CreateFederacionWithAdminAsync(SaaSCreateFederacionDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try 
            {
                var fed = new Entidades.Entidades.Federacion
                {
                    Nombre = dto.Nombre,
                    Sigla = dto.Sigla,
                    Email = dto.Email,
                    Telefono = dto.Telefono,
                    Direccion = dto.Direccion,
                    Activo = true,
                    PlanSaaSId = 1,
                    FechaAltaPlan = DateTime.UtcNow.Date,
                    FechaVencimientoPlan = DateTime.UtcNow.Date.AddMonths(1)
                };
                
                _context.Federaciones.Add(fed);
                await _context.SaveChangesAsync();

                var user = new Entidades.Entidades.Usuario
                {
                    Username = dto.AdminUsername.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.AdminPassword),
                    Email = dto.AdminEmail, 
                    Rol = "Admin",
                    FederacionId = fed.Id,
                    Activo = true
                };

                _context.Usuarios.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return fed.Id;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                var innerMsg = ex.InnerException?.Message ?? "";
                
                string userFriendlyMessage = "Error interno al guardar los datos.";
                
                if (innerMsg.Contains("23505") || innerMsg.Contains("duplicate key"))
                {
                    if (innerMsg.Contains("IX_Usuarios_Username"))
                        userFriendlyMessage = "El nombre de usuario administrador ya está en uso. Por favor, elige otro.";
                    else if (innerMsg.Contains("IX_Usuarios_Email"))
                        userFriendlyMessage = "El email del administrador ya está registrado en otra cuenta. Debe ser único.";
                    else if (innerMsg.Contains("IX_Federaciones_Nombre") || innerMsg.Contains("IX_Clubes_Nombre"))
                        userFriendlyMessage = "Ya existe una federación o club con ese nombre.";
                    else
                        userFriendlyMessage = "Un dato ingresado ya existe en el sistema y no puede duplicarse.";
                        
                    throw new Exception(userFriendlyMessage);
                }

                throw new Exception($"Error al crear la federación: {ex.Message}");
            }
        }

        public async Task<GlobalMetricsDto> GetGlobalMetricsAsync()
        {
            var federaciones = await _context.Federaciones.ToListAsync();
            var totalAtletas = await _context.Participantes.CountAsync();
            var totalClubes = await _context.Clubes.CountAsync();
            var torneosActivos = await _context.Eventos.CountAsync(e => e.Estado != EstadoEventoEnum.Finalizado);

            var crecimiento = new List<MonthlyGrowthDto>
            {
                new MonthlyGrowthDto { Mes = "Ene", Cantidad = 5 },
                new MonthlyGrowthDto { Mes = "Feb", Cantidad = 8 },
                new MonthlyGrowthDto { Mes = "Mar", Cantidad = 12 },
                new MonthlyGrowthDto { Mes = "Abr", Cantidad = 18 },
                new MonthlyGrowthDto { Mes = "May", Cantidad = 25 }
            };

            return new GlobalMetricsDto
            {
                TotalFederaciones = federaciones.Count,
                TotalClubesAfiliados = totalClubes,
                TotalAtletasGlobales = totalAtletas,
                TorneosActivosGlobales = torneosActivos,
                CrecimientoMensual = crecimiento,
                TopFederaciones = federaciones
                    .Select(f => new FederacionMetricDto { 
                        Nombre = f.Nombre,
                        ClubesCount = _context.Clubes.Count(c => c.FederacionId == f.Id)
                    })
                    .OrderByDescending(f => f.ClubesCount)
                    .Take(5)
                    .ToList()
            };
        }
    }
}
