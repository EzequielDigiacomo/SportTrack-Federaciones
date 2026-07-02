using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class DelegadoClub
    {
        [Key] // ?? ¡Obligatorio!
        [ForeignKey(nameof(Participante))]
        public int? IdParticipante { get; set; }

        public virtual Participante Participante { get; set; } = null!;

        [ForeignKey(nameof(Rol))]
        public int IdRol { get; set; }
        public virtual Rol Rol { get; set; } = null!;

        [ForeignKey(nameof(Federacion))]
        public int? FederacionId { get; set; }
        public virtual Federacion Federacion { get; set; } = null!;

        [ForeignKey(nameof(Club))]
        public int? ClubIdClub { get; set; }
        public virtual Club? Club { get; set; } = null!;
    }
}
