using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.Entidades
{
    public class Prueba
    {
        public int IdPrueba { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int TipoBote { get; set; }
        public int CategoriaEdad { get; set; }
        public int DistanciaId { get; set; }
        public int SexoCompetencia { get; set; }
        public string? Descripcion { get; set; }

        // Navigation properties
        public Bote Bote { get; set; } = null!;
        public Categoria Categoria { get; set; } = null!;
        public Distancia Distancia { get; set; } = null!;
        public Sexo Sexo { get; set; } = null!;
        public ICollection<EventoPrueba> EventoPruebas { get; set; } = new List<EventoPrueba>();
    }

}
