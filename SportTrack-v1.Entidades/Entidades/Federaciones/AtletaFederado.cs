using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class AtletaFederado
    {
        [Key]
        public int ParticipanteId { get; set; }

        [ForeignKey(nameof(ParticipanteId))]
        public virtual Participante Participante { get; set; } = null!;

        [ForeignKey("Club")]
        public int? IdClub { get; set; }
        [JsonIgnore]
        public virtual Club? Club { get; set; } = null!;

        public int? IdFederacion { get; set; }
        [ForeignKey(nameof(IdFederacion))]
        public virtual Federacion? Federacion { get; set; }

        public EstadoPago EstadoPago { get; set; }

        public bool PerteneceSeleccion { get; set; }

        // 👇 Renombrado: CategoriaSeleccion → Categoria
        public CategoriaEdad? Categoria { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }
        public DateTime? FechaAptoMedico { get; set; }

        [JsonIgnore]
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
        [JsonIgnore]
        public virtual ICollection<AtletaTutor> Tutores { get; set; } = new List<AtletaTutor>();
    }
}
