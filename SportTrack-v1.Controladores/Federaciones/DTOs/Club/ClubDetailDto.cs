using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportTrack_v1.Entidades.DTOs.AtletaFederado;
using SportTrack_v1.Entidades.DTOs.Entrenador;
using SportTrack_v1.Entidades.DTOs.DelegadoClub;
using SportTrack_v1.Entidades.DTOs.PagoTransaccion;

namespace SportTrack_v1.Entidades.DTOs.Club
{
    public class ClubDetailDto
    {
        public int IdClub { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Siglas { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int? IdFederacion { get; set; }
        public Enums.EstadoPago EstadoMatricula { get; set; }

        // InformaciÃ³n relacionada
        public List<AtletaDto>? AtletasFederados { get; set; }
        public List<EntrenadorDto>? Entrenadores { get; set; }
        public List<DelegadoClubDto>? Representantes { get; set; }
        public List<PagoTransaccionDto>? Pagos { get; set; }
    }
}
