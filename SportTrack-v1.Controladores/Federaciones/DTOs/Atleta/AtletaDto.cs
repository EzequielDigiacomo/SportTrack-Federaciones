using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.AtletaFederado
{
    public class AtletaDto
    {
        public int ParticipanteId { get; set; }
        public int? IdClub { get; set; }
        public string Documento { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public EstadoPago EstadoPago { get; set; }
        public bool PerteneceSeleccion { get; set; }
        public CategoriaEdad? Categoria { get; set; }
        public bool BecadoEnard { get; set; }
        public bool BecadoSdn { get; set; }
        public decimal MontoBeca { get; set; }
        public bool PresentoAptoMedico { get; set; }
        public DateTime? FechaAptoMedico { get; set; }

        public DateTime FechaCreacion { get; set; }
        public string? NombrePersona { get; set; }
        public string? NombreClub { get; set; }
    }
}
