using Microsoft.AspNetCore.Mvc;

namespace CeciMongo.Domain.DTO.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing the identifier of a user.
    /// </summary>
    public class UserIdentifierDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        [BindProperty(Name = "userId")]
        public string UserId { get; set; }
    }
}
