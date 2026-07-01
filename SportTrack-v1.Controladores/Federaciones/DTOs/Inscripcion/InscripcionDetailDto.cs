using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Evento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Inscripcion
{
    public class InscripcionDetailDto
    {
        public int IdInscripcion { get; set; }
        public int ParticipanteId { get; set; }
        public int IdEvento { get; set; }
        public int IdEventoPrueba { get; set; }
        public DateTime FechaInscripcion { get; set; }

        public AtletaDto? AtletaFederado { get; set; }
        public EventoDto? Evento { get; set; }
    }
}
