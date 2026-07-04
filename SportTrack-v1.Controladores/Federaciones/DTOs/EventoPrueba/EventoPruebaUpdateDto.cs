using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Entidades.DTOs.EventoPrueba
{
    public class EventoPruebaUpdateDto : EventoPruebaCreateDto
    {
        [Required]
        public int IdEventoPrueba { get; set; }
    }
}
