using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.Federacion;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.DelegadoClub
{
    public class DelegadoClubDetailDto
    {
        public int? ParticipanteId { get; set; }
        public int IdRol { get; set; }
        public int? IdFederacion { get; set; }
        public int? IdClub { get; set; }

        public PersonaDto? Participante { get; set; }
        public RolDto? Rol { get; set; }
        public FederacionDto? Federacion { get; set; }
        public ClubDto? Club { get; set; }
    }
}
