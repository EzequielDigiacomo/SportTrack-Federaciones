using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Federacion
    {
        [Key]
        public int IdFederacion { get; set; }
        
        public string? Sigla { get; set; }
        public bool Activo { get; set; } = true;

        [Required, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public string Cuit { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Telefono { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(100)]
        public string BancoNombre { get; set; } = string.Empty;

        [MaxLength(50)]
        public string TipoCuenta { get; set; } = string.Empty;

        [MaxLength(50)]
        public string NumeroCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string TitularCuenta { get; set; } = string.Empty;

        [MaxLength(100)]
        public string EmailCobro { get; set; } = string.Empty;

        public virtual ICollection<DelegadoClub> DelegadosClub { get; set; } = new List<DelegadoClub>();
        public virtual ICollection<Club> Clubes { get; set; } = new List<Club>();
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public virtual ICollection<Evento> Eventos { get; set; } = new List<Evento>();
        public virtual ICollection<AtletaFederado> AtletasFederados { get; set; } = new List<AtletaFederado>();
        public virtual ICollection<Entrenador> Entrenadores { get; set; } = new List<Entrenador>();

        // SaaS Plan
        public int? PlanSaaSId { get; set; }
        public PlanSaaS? PlanSaaS { get; set; }

        public DateTime? FechaAltaPlan { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public string? FrecuenciaPago { get; set; } // "Mensual", "Anual"
        public bool BloqueadaPorFaltaDePago { get; set; } = false;
    }
}
