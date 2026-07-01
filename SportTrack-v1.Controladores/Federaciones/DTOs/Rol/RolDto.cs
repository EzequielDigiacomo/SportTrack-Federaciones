using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Rol
{
    public class RolDto
    {
        public int IdRol { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public RolTipo? TipoEnum { get; set; } // ?? Agregar esta propiedad
        // Estadísticas para mostrar en listas
        public int CantidadRepresentantes { get; set; }
    }
}
