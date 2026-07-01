using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.Participante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Usuario
{
    public class UsuarioDetailDto
    {
        public int IdUsuario { get; set; }
        public int? ParticipanteId { get; set; }
        public int? IdFederacion { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool EstaActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimoAcceso { get; set; }
        public int? IdClub { get; set; }
        public string? Rol { get; set; } = string.Empty;

        public string NombreClub { get; set; } = string.Empty;
        public string Emailclub { get; set; } = string.Empty;

        // Asegúrate de tener esta clase PersonaDto
        public PersonaDto? Participante { get; set; }

        public ClubDetailDto? Club { get; set; }
    }
}
