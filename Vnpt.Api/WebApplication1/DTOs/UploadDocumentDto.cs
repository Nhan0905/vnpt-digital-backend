using Microsoft.AspNetCore.Http;

namespace Vnpt.Api.DTOs
{
    public class UploadDocumentDto
    {
        public required IFormFile File { get; set; }
    }
}