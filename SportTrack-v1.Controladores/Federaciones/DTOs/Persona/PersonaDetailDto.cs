using SportTrack_v1.Entidades.Entidades;
using SIGDEF.DTOs;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.PagoTransaccion;
using SportTrack_v1.Entidades.DTOs.Tutor;
using SportTrack_v1.Entidades.DTOs.Usuario;
using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Participante
{
    public class PersonaDetailDto
    {
        public int ParticipanteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Documento { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public Sexo Sexo { get; set; } // NUEVO
        public string SexoDisplay { get; set; } = string.Empty; // Para mostrar en UI
        // Información relacionada
        public UsuarioDto? Usuario { get; set; }
        public DelegadoClubDto? DelegadoClub { get; set; }
        public EntrenadorDto? Entrenador { get; set; }
        public TutorDto? Tutor { get; set; }
        public AtletaDto? AtletaFederado { get; set; }
        public List<PagoTransaccionDto>? Pagos { get; set; }
    }
}
