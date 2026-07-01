using SportTrack_v1.Entidades.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Entidades.DTOs.Prueba
{
    public class PruebaCreateDto
    {
        [Required]
        public DistanciaRegata Distancia { get; set; }
        [Required]
        public CategoriaEdad CategoriaEdad { get; set; }
        [Required]
        public SexoCompetencia SexoCompetencia { get; set; }
        [Required]
        public TipoBote TipoBote { get; set; }
    }
}
