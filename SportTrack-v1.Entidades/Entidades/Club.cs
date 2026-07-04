using System;
using System.Collections.Generic;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Club
    {
        public int IdClub { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Siglas { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public string? Ubicacion { get; set; }
        public bool Activo { get; set; } = true;
        
        // RelaciÃ³n con la FederaciÃ³n a la que pertenece
        public int? IdFederacion { get; set; }
        public Federacion? Federacion { get; set; }
        
        // SaaS Plan
        public int? PlanSaaSId { get; set; }
        public PlanSaaS? PlanSaaS { get; set; }

        // Subscription / Payment Status
        public string? FrecuenciaPago { get; set; } // "Mensual", "Anual"
        public DateTime? FechaAltaPlan { get; set; }
        public DateTime? FechaVencimientoPlan { get; set; }
        public bool BloqueadoPorFaltaDePago { get; set; } = false;
        public bool PagoAfiliacionAlDia { get; set; } = true;
        public bool SolicitudPagoPendiente { get; set; } = false;

        // Navigation properties
        public ICollection<Participante> Participantes { get; set; } = new List<Participante>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
