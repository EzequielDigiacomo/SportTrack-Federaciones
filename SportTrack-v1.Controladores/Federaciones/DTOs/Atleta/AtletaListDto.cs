using SportTrack_v1.Entidades.Enums;
using System;

namespace SportTrack_v1.Entidades.DTOs.AtletaFederado
{
    public class AtletaListDto
    {
        public int ParticipanteId { get; set; }
        public string NombrePersona { get; set; } = "";
        public string? Documento { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int? Edad { get; set; }
        public string? NombreClub { get; set; }
        public CategoriaEdad? Categoria { get; set; }
        public bool PerteneceSeleccion { get; set; }
        public EstadoPago EstadoPago { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? CantidadDocumentos { get; set; }
        public TutorListDto? TutorInfo { get; set; }
    }

    public class TutorListDto
    {
        public int ParticipanteId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Documento { get; set; }
        public string? Telefono { get; set; }
    }
}
