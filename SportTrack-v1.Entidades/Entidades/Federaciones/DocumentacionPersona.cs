using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportTrack_v1.Entidades.Entidades
{
    public class DocumentacionPersona
    {
        [Key]
        public int Id { get; set; }
        public int? PersonaId { get; set; }
        // Aquí conectamos con la entidad BASE "Participante"
        [ForeignKey("PersonaId")]
        public virtual Participante Participante { get; set; } = null!;
        public int? TipoDocumento { get; set; } // Enum (DNI, Pasaporte, etc)
        [Required]
        public string UrlArchivo { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? PublicId { get; set; } // ID de Cloudinary
        public DateTime FechaCarga { get; set; } = DateTime.UtcNow;
    }
}
