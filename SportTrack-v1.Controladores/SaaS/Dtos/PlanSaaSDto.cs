namespace SportTrack_v1.Controladores.SaaS.Dtos
{
    public class PlanSaaSDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int MaxAtletas { get; set; }
        public int MaxTorneosActivos { get; set; }
        public bool ResultadosTiempoReal { get; set; }
        public bool ExportacionExcel { get; set; }
        public bool SoportePrioritario { get; set; }

        // Flags de acceso derivados del nombre del plan (sin migración de BD)
        public bool AccesoSigdef => Nombre.Contains("SIGDEF", System.StringComparison.OrdinalIgnoreCase)
                                 || Nombre.Contains("Dúo", System.StringComparison.OrdinalIgnoreCase);

        public bool AccesoSportTrack => Nombre.Contains("SportTrack", System.StringComparison.OrdinalIgnoreCase)
                                     || Nombre.Contains("Dúo", System.StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Acceso a paneles de control en vivo (Largador, Cronometrista, Juez de Control).
        /// Solo disponible en planes de talla L.
        /// </summary>
        public bool AccesoControlesLive => Nombre.EndsWith("(L)", System.StringComparison.OrdinalIgnoreCase);
    }
}
