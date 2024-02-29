using Microsoft.AspNetCore.Http;

namespace CeciMongo.Domain.DTO.Import
{
    /// <summary>
    /// Data Transfer Object (DTO) representing a file upload for import.
    /// </summary>
    public class FileUploadDTO
    {
        /// <summary>
        /// Gets or sets the file for import.
        /// </summary>
        public IFormFile File { get; set; }
    }
}