using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Entrenador
{
    public class EntrenadorCreateDto
    {
        [Required(ErrorMessage = "El ID de la Participante es requerido")]
        public int ParticipanteId { get; set; }

        public int? IdClub { get; set; }
        public int? IdFederacion { get; set; }

        //[Required(ErrorMessage = "El campo PerteneceSeleccion es requerido")]
        public bool PerteneceSeleccion { get; set; }

        //[Required(ErrorMessage = "La categoría de selección es requerida")]
        //[MaxLength(50, ErrorMessage = "La categoría de selección no puede exceder 50 caracteres")]
        public string? CategoriaSeleccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo BecadoEnard es requerido")]
        public bool BecadoEnard { get; set; }

        [Required(ErrorMessage = "El campo BecadoSdn es requerido")]
        public bool BecadoSdn { get; set; }

        [Required(ErrorMessage = "El monto de la beca es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El monto de la beca debe ser mayor o igual a 0")]
        public decimal MontoBeca { get; set; }

        [Required(ErrorMessage = "El campo PresentoAptoMedico es requerido")]
        public bool PresentoAptoMedico { get; set; }
    }

}
