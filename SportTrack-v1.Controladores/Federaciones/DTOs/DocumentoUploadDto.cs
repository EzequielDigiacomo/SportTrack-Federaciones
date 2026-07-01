using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SportTrack_v1.Entidades.DTOs
{
    public class DocumentoUploadDto
    {
        public IFormFile File { get; set; }
        public int PersonaId { get; set; }
        public int TipoDocumento { get; set; }
    }
}
