using System;
using System.Collections.Generic;
using System.Linq;

namespace SportTrack_v1.Entidades.DTOs.Evento
{
    public class EventoResponseDto
    {
        public int IdEvento { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // ?? TIPO DE EVENTO
        public int TipoEventoId { get; set; }
        public string TipoEventoNombre { get; set; } = string.Empty;
        public string TipoEventoIcono { get; set; } = string.Empty;
        public string TipoEventoColor { get; set; } = string.Empty;

        // ?? FECHAS
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public DateTime? FechaInicioInscripciones { get; set; }
        public DateTime? FechaFinInscripciones { get; set; }

        // ?? UBICACIÓN
        public string? Ubicacion { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }

        // ?? PRUEBAS (LISTA DE CARRERAS)
        public List<EventoPruebaResponseDto> Pruebas { get; set; } = new List<EventoPruebaResponseDto>();

        // ?? PROPIEDADES PARA COMPATIBILIDAD
        public int DistanciaId => Pruebas.FirstOrDefault()?.DistanciaId ?? 0;
        public string DistanciaCodigo => Pruebas.FirstOrDefault()?.DistanciaCodigo ?? string.Empty;
        public string DistanciaNombre => Pruebas.FirstOrDefault()?.DistanciaNombre ?? string.Empty;
        public decimal DistanciaMetros => Pruebas.FirstOrDefault()?.Metros ?? 0;
        public string DistanciasDisplay => string.Join(", ", Pruebas.Select(d => d.DistanciaCodigo));

        // ?? CONFIGURACIÓN
        public decimal PrecioBase { get; set; }
        public int CupoMaximo { get; set; }
        public bool TieneCronometraje { get; set; }
        public bool RequiereCertificadoMedico { get; set; }

        // ?? ESTADO
        public bool EstaActivo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string? Observaciones { get; set; }

        // ?? ESTADÍSTICAS
        public int TotalInscritos { get; set; }
        public int CuposDisponibles { get; set; }
        public bool InscripcionesAbiertas { get; set; }
        public bool TieneCupoDisponible { get; set; }
        public int DiasRestantes { get; set; }

        // ?? PROPIEDADES CALCULADAS PARA FRONTEND
        public string FechasDisplay { get; set; } = string.Empty;
        public string PeriodoInscripcionesDisplay { get; set; } = string.Empty;
        public string EstadoDisplay { get; set; } = string.Empty;
        public string UbicacionCompleta { get; set; } = string.Empty;
        public string PrecioDisplay { get; set; } = string.Empty;
        public string CupoDisplay { get; set; } = string.Empty;

        // ?? MÉTODO PRINCIPAL
        public static EventoResponseDto FromEntity(Entidades.Evento evento)
        {
            if (evento == null)
                return null;

            var dto = new EventoResponseDto
            {
                // ?? INFORMACIÓN BÁSICA
                IdEvento = evento.IdEvento,
                Nombre = evento.Nombre,
                Descripcion = evento.Descripcion,

                // ?? TIPO DE EVENTO
                TipoEventoId = (int)evento.TipoEvento,
                TipoEventoNombre = GetTipoEventoDisplay(evento.TipoEvento),
                TipoEventoIcono = GetTipoEventoIcono(evento.TipoEvento),
                TipoEventoColor = GetTipoEventoColor(evento.TipoEvento),

                // ?? FECHAS
                FechaInicio = evento.FechaInicio,
                FechaFin = evento.FechaFin,
                FechaInicioInscripciones = evento.FechaInicioInscripciones,
                FechaFinInscripciones = evento.FechaFinInscripciones,

                // ?? UBICACIÓN
                Ubicacion = evento.Ubicacion,
                Ciudad = evento.Ciudad,
                Provincia = evento.Provincia,

                // ?? PRUEBAS (MÚLTIPLES)
                Pruebas = evento.Pruebas?.Select(ed => EventoPruebaResponseDto.FromEntity(ed)).ToList() ?? new List<EventoPruebaResponseDto>(),

                // ?? CONFIGURACIÓN
                PrecioBase = evento.PrecioBase,
                CupoMaximo = evento.CupoMaximo,
                TieneCronometraje = evento.TieneCronometraje,
                RequiereCertificadoMedico = evento.RequiereCertificadoMedico,

                // ?? ESTADO
                EstaActivo = evento.EstaActivo,
                FechaCreacion = evento.FechaCreacion,
                Observaciones = evento.Observaciones,

                // ?? ESTADÍSTICAS
                TotalInscritos = evento.Inscripciones?.Count ?? 0,
                CuposDisponibles = evento.CupoMaximo - (evento.Inscripciones?.Count ?? 0),
                DiasRestantes = (evento.FechaInicio - DateTime.UtcNow).Days
            };

            // ?? CALCULAR PROPIEDADES DERIVADAS
            dto.InscripcionesAbiertas = CalcularInscripcionesAbiertas(evento);
            dto.TieneCupoDisponible = dto.CuposDisponibles > 0;

            // ?? STRINGS FORMATEADOS PARA FRONTEND
            dto.FechasDisplay = $"{dto.FechaInicio:dd/MM/yyyy} - {dto.FechaFin:dd/MM/yyyy}";

            dto.PeriodoInscripcionesDisplay = dto.FechaInicioInscripciones.HasValue && dto.FechaFinInscripciones.HasValue
                ? $"{dto.FechaInicioInscripciones:dd/MM/yyyy} - {dto.FechaFinInscripciones:dd/MM/yyyy}"
                : "Sin período definido";

            dto.EstadoDisplay = dto.EstaActivo
                ? (dto.InscripcionesAbiertas ? "?? Inscripciones Abiertas" : "?? Próximamente")
                : "?? Finalizado";

            dto.UbicacionCompleta = string.Join(", ",
                new[] { dto.Ubicacion, dto.Ciudad, dto.Provincia }
                    .Where(x => !string.IsNullOrEmpty(x)));

            dto.PrecioDisplay = dto.PrecioBase > 0
                ? $"${dto.PrecioBase:N2}"
                : "Gratuito";

            dto.CupoDisplay = $"{dto.TotalInscritos}/{dto.CupoMaximo} inscritos";

            return dto;
        }

        // Métodos auxiliares para TipoEvento
        private static string GetTipoEventoDisplay(Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Enums.TipoEvento.CarreraOficial => "Carrera Oficial",
                Enums.TipoEvento.Campeonato => "Campeonato",
                Enums.TipoEvento.Recreativo => "Recreativo",
                Enums.TipoEvento.Entrenamiento => "Entrenamiento",
                Enums.TipoEvento.Clasificatorio => "Clasificatorio",
                _ => tipo.ToString()
            };
        }

        private static string GetTipoEventoIcono(Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Enums.TipoEvento.CarreraOficial => "??",
                Enums.TipoEvento.Campeonato => "??",
                Enums.TipoEvento.Recreativo => "??",
                Enums.TipoEvento.Entrenamiento => "?",
                Enums.TipoEvento.Clasificatorio => "??",
                _ => "??"
            };
        }

        private static string GetTipoEventoColor(Enums.TipoEvento tipo)
        {
            return tipo switch
            {
                Enums.TipoEvento.CarreraOficial => "#FF6B6B",
                Enums.TipoEvento.Campeonato => "#4ECDC4",
                Enums.TipoEvento.Recreativo => "#45B7D1",
                Enums.TipoEvento.Entrenamiento => "#96CEB4",
                Enums.TipoEvento.Clasificatorio => "#FECA57",
                _ => "#C8D6E5"
            };
        }

        private static bool CalcularInscripcionesAbiertas(Entidades.Evento evento)
        {
            var ahora = DateTime.UtcNow;
            var inicioOk = !evento.FechaInicioInscripciones.HasValue || ahora >= evento.FechaInicioInscripciones;
            var finOk = !evento.FechaFinInscripciones.HasValue || ahora <= evento.FechaFinInscripciones;
            return inicioOk && finOk && evento.EstaActivo;
        }
    }


}
