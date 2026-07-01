using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.Tutor;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Entidades.DTOs.AtletaTutor
{
    public class AtletaTutorDetailDto
    {
        public int ParticipanteId { get; set; }
        public int IdTutor { get; set; }
        public Parentesco Parentesco { get; set; }
        public AtletaDto? AtletaFederado { get; set; }
        public TutorDto? Tutor { get; set; }
    }
}