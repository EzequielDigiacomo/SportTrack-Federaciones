using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Inscripcion
{
    public class InscripcionCreateDto
    {
        [Required(ErrorMessage = "El ID del AtletaFederado es requerido")]
        public int ParticipanteId { get; set; }

        [Required(ErrorMessage = "El ID de la prueba es requerido")]
        public int IdEventoPrueba { get; set; }

        public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    }
}
