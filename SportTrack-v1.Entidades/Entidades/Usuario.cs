using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = "Club"; // "Admin", "Club", "Largador", "Cronometrista", "Federacion"
        
        public int? IdClub { get; set; }
        public Club? Club { get; set; }

        public int? IdFederacion { get; set; }
        public Federacion? Federacion { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public bool EstaActivo { get; set; } = true;
        public int IntentosFallidos { get; set; } = 0;

        // Datos personales — para identificación y auditoría de jueces
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Dni { get; set; }
        public string? Telefono { get; set; }

        public int? ParticipanteId { get; set; }
        [ForeignKey(nameof(ParticipanteId))]
        public virtual Participante? Participante { get; set; }
    }
}
