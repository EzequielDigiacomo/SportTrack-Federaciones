using SportTrack_v1.Entidades.DTOs.AtletaTutor;
using SportTrack_v1.Entidades.DTOs.Participante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Tutor
{
    public class TutorDetailDto
    {
        public int ParticipanteId { get; set; }
        public string TipoTutor { get; set; } = string.Empty;

        public PersonaDto? Participante { get; set; }
        public List<AtletaTutorDto>? AtletasTutores { get; set; }
    }
}
