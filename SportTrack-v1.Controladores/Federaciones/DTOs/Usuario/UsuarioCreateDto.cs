using SportTrack_v1.Entidades.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportTrack_v1.Entidades.DTOs.Usuario
{
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El ID de la Participante es requerido")]
        public int? ParticipanteId { get; set; }

        [Required(ErrorMessage = "El ID del club es requerido")]
        public int? IdClub { get; set; }

        public int? IdFederacion { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(50, ErrorMessage = "El nombre de usuario no puede exceder 50 caracteres")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [MaxLength(100, ErrorMessage = "La contraseña no puede exceder 100 caracteres")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool EstaActivo { get; set; } = true;

        [Required(ErrorMessage = "El rol es requerido")]
        [RegularExpression("^(Admin|Club|AtletaFederado|Entrenador|Usuario)$",
         ErrorMessage = "Rol inválido. Valores permitidos: Admin, Club, AtletaFederado, Entrenador, Usuario")]
        public string Rol { get; set; } = RolTipo.DelegadoClub.ToString();
    }
}
