using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.AtletaTutor
{
    public class AtletaTutorCreateDto
    {
        public int ParticipanteId { get; set; }
        public int IdTutor { get; set; }
        public Parentesco Parentesco { get; set; }
    }
}