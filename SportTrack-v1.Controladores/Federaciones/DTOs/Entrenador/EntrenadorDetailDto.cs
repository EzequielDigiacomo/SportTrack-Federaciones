using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.DTOs.Participante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Entrenador
{
    public class EntrenadorDetailDto
    {
        public int ParticipanteId { get; set; }
        public int? IdClub { get; set; }
        public bool PerteneceSeleccion { get; set; }
        public string? CategoriaSeleccion { get; set; } = string.Empty;
        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }

        public PersonaDto? Participante { get; set; }
        public ClubDto? Club { get; set; }
    }
}
