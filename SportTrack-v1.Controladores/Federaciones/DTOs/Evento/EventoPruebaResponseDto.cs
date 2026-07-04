using System;
using System.Collections.Generic;
using System.Linq;

namespace SportTrack_v1.Entidades.DTOs.Evento
{
    // ðŸ”¹ DTO PARA PRUEBAS DEL EVENTO (RESPUESTA)
    public class EventoPruebaResponseDto
    {
        public int IdEventoPrueba { get; set; }
        public int DistanciaId { get; set; }
        public string DistanciaCodigo { get; set; } = string.Empty;
        public string DistanciaNombre { get; set; } = string.Empty;
        public decimal Metros { get; set; }
        public int CategoriaEdad { get; set; }
        public decimal? PrecioCategoria { get; set; }
        public int DistanciaRegata { get; set; }
        public int TipoBote { get; set; }
        public string TipoBoteNombre { get; set; } = string.Empty;
        public int SexoCompetencia { get; set; }

        public static EventoPruebaResponseDto FromEntity(Entidades.EventoPrueba eventoPrueba)
        {
            if (eventoPrueba?.Prueba == null) return new EventoPruebaResponseDto();

            return new EventoPruebaResponseDto
            {
                IdEventoPrueba = eventoPrueba.IdEventoPrueba,
                DistanciaId = (int)eventoPrueba.Prueba.Distancia,
                DistanciaCodigo = GetDistanciaDisplay(eventoPrueba.Prueba.Distancia),
                DistanciaNombre = eventoPrueba.Prueba.Distancia.ToString(),
                Metros = GetDistanciaMetros(eventoPrueba.Prueba.Distancia),
                CategoriaEdad = (int)eventoPrueba.Prueba.CategoriaEdad,
                PrecioCategoria = eventoPrueba.PrecioCategoria,
                TipoBote = (int)eventoPrueba.Prueba.TipoBote,
                TipoBoteNombre = eventoPrueba.Prueba.TipoBote.ToString(),
                SexoCompetencia = (int)eventoPrueba.Prueba.SexoCompetencia,
                DistanciaRegata = (int)eventoPrueba.Prueba.Distancia
            };
        }

        private static string GetDistanciaDisplay(SportTrack_v1.Entidades.Enums.DistanciaRegata distancia)
        {
            return distancia switch
            {
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DoscientosMetros => "200m",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TrecientosCincuentaMetros => "350m",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuatroCientosMetros => "400m",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuinientosMetros => "500m",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.MilMetros => "1000m",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DosKilometros => "2K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TresKilometros => "3K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.CincoKilometros => "5K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DiezKilometros => "10K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuinceKilometros => "15K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.VeintiDosKilometros => "22K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.VeintiCincoKilometros => "25K",
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TreintaDosKilometros => "32K",
                _ => distancia.ToString()
            };
        }

        private static decimal GetDistanciaMetros(SportTrack_v1.Entidades.Enums.DistanciaRegata distancia)
        {
            return distancia switch
            {
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DoscientosMetros => 200,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TrecientosCincuentaMetros => 350,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuatroCientosMetros => 400,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuinientosMetros => 500,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.MilMetros => 1000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DosKilometros => 2000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TresKilometros => 3000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.CincoKilometros => 5000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.DiezKilometros => 10000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.QuinceKilometros => 15000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.VeintiDosKilometros => 22000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.VeintiCincoKilometros => 25000,
                SportTrack_v1.Entidades.Enums.DistanciaRegata.TreintaDosKilometros => 32000,
                _ => 0
            };
        }
    }
}
