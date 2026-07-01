using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.Participante;
using SportTrack_v1.Entidades.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Entidades.DTOs.AtletaFederado
{
    public class AtletaFullCreateDto
    {
        [Required]
        public PersonaCreateDto PersonaAtleta { get; set; } = new();

        [Required]
        public AtletaCreateDto DatosDeportivos { get; set; } = new();

        public bool EsMenor { get; set; }

        public TutorFullDto? Tutor { get; set; }
    }

    public class TutorFullDto
    {
        public int? IdPersonaTutor { get; set; } // Si ya existe la Participante
        public PersonaCreateDto? PersonaTutor { get; set; } // Si hay que crearla
        
        [Required]
        public int Parentesco { get; set; } // Enum o ID de parentesco
    }
}
