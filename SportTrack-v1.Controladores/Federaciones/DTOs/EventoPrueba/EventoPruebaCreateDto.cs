using SportTrack_v1.Entidades.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Entidades.DTOs.EventoPrueba
{
    public class EventoPruebaCreateDto
    {
       public int IdEvento { get; set; }
        [Required(ErrorMessage = "La distancia es requerida")]
        public int IdPrueba { get; set; }
        
        public decimal? PrecioCategoria { get; set; }
    }
}
