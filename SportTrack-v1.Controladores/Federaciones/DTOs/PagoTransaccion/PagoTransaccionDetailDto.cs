using SportTrack_v1.Entidades.DTOs.Club;
using SportTrack_v1.Entidades.Enums;
using SportTrack_v1.Entidades.DTOs.Participante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.PagoTransaccion
{
    public class PagoTransaccionDetailDto
    {
        public int IdPago { get; set; }
        public string Concepto { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public EstadoPagoTransaccion Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public int ParticipanteId { get; set; }
        public int IdClub { get; set; }
        public string IdMercadoPago { get; set; } = string.Empty;

        public PersonaDto? Participante { get; set; }
        public ClubDto? Club { get; set; }
    }
}
