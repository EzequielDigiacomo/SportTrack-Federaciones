using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Club
{
    public class ClubDto
    {
        public int IdClub { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public int? IdFederacion { get; set; }
        public SportTrack_v1.Entidades.Enums.EstadoPago EstadoMatricula { get; set; }
        public bool TieneDelegado { get; set; }
        // Estadísticas para mostrar en listas
        public int CantidadAtletas { get; set; }
        public int CantidadEntrenadores { get; set; }
        public int CantidadRepresentantes { get; set; }
        public Dictionary<int, int>? AtletasPorCategoria
        {
            get; set;
        }
    }
}
