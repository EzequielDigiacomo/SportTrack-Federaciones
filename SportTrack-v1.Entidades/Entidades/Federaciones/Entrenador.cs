using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Entrenador
    {
        [Key]
        [ForeignKey(nameof(Participante))]
        public int ParticipanteId { get; set; }

        public virtual Participante Participante { get; set; } = null!;

        [ForeignKey(nameof(Club))]
        public int? IdClub { get; set; }
        public virtual Club? Club { get; set; } = null!;

        public int? IdFederacion { get; set; }
        [ForeignKey(nameof(IdFederacion))]
        public virtual Federacion? Federacion { get; set; }

        [MaxLength(50)]

        public bool? PerteneceSeleccion { get; set; }
        [MaxLength(50)]
        public string? CategoriaSeleccion { get; set; } = string.Empty;

        public bool? BecadoEnard { get; set; }
        public bool? BecadoSdn { get; set; }
        public decimal? MontoBeca { get; set; }
        public bool? PresentoAptoMedico { get; set; }
    }
}
