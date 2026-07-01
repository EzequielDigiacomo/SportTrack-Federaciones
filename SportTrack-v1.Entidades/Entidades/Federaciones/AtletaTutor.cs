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
    public class AtletaTutor
    {
        [Key, Column(Order = 0)]
        public int IdAtleta { get; set; }

        [Key, Column(Order = 1)]
        public int IdTutor { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(IdAtleta))]
        public virtual AtletaFederado AtletaFederado { get; set; } = null!;
        [JsonIgnore]
        [ForeignKey(nameof(IdTutor))]
        public virtual Tutor Tutor { get; set; } = null!;

        public Parentesco Parentesco { get; set; }
    }
}
