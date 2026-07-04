using SportTrack_v1.Controladores.Extensions;
using SportTrack_v1.Controladores.Helpers;
using SportTrack_v1.Entidades.Enums;

namespace SportTrack_v1.Entidades.DTOs.Prueba
{
    public class PruebaDto
    {
        public int IdPrueba { get; set; }
        public DistanciaRegata Distancia { get; set; }
        public CategoriaEdad CategoriaEdad { get; set; }
        public SexoCompetencia SexoCompetencia { get; set; }
        public TipoBote TipoBote { get; set; }

        public string DistanciaDisplay => Distancia.ToDisplayString();
        public string CategoriaDisplay => CategoriaEdad.ToString(); // Or custom helper
        public string SexoDisplay => SexoCompetencia.ToString();
        public string BoteDisplay => TipoBote.ToString();

        public string NombreCompleto => $"{Distancia.ToDisplayString()} {TipoBote} {CategoriaEdad} {SexoCompetencia}";
    }
}
