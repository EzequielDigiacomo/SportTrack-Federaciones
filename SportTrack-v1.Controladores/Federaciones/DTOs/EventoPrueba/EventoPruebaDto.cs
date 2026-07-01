namespace SportTrack_v1.Entidades.DTOs.EventoPrueba
{
    public class EventoPruebaDto
    {
        public int IdEventoPrueba { get; set; }
        public int IdEvento { get; set; }
        public int IdPrueba { get; set; }
        public decimal? PrecioCategoria { get; set; }
        
        public SportTrack_v1.Entidades.DTOs.Prueba.PruebaDto Prueba { get; set; }
    }
}
