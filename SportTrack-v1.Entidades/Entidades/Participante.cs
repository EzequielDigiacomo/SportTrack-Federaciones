using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Participante
    {
        public int ParticipanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public int SexoId { get; set; }
        public int? CategoriaId { get; set; }
        public string? Pais { get; set; }
        public int? IdClub { get; set; }
        public Club? Club { get; set; }
        public string? Documento { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? Direccion { get; set; }
        public bool PagoAfiliacionAlDia { get; set; } = true;

        // Propiedades calculadas
        public int Edad => DateTime.UtcNow.Year - FechaNacimiento.Year;

        // Navigation properties
        public Sexo Sexo { get; set; } = null!;
        public Categoria? Categoria { get; set; }
        public ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();

        // Relaciones de Federación (SIGDEF)
        public virtual DelegadoClub? DelegadoClub { get; set; }
        public virtual Entrenador? Entrenador { get; set; }
        public virtual Tutor? Tutor { get; set; }
        public virtual AtletaFederado? AtletaFederado { get; set; }
        public virtual ICollection<DocumentacionPersona> Documentacion { get; set; } = new List<DocumentacionPersona>();
    }
}
