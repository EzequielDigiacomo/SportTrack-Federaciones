using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Participante
{
    public class PersonaDto
    {
        public int? ParticipanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string? Email { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public Sexo Sexo { get; set; } // NUEVO
        public string SexoDisplay { get; set; } = string.Empty; // Para mostrar en UI

        // Información adicional para mostrar
        public int? Edad { get; set; }
        public string? NombreCompleto { get; set; }
        public string? TipoPersona { get; set; } // AtletaFederado, Entrenador, Tutor, etc.
    }
}
