using Microsoft.AspNetCore.Mvc;

namespace CeciMongo.Domain.DTO.Role
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an identifier for deleting a role.
    /// </summary>
    public class RoleDeleteDTO
    {
        /// <summary>
        /// Gets or sets the identifier of the role to be deleted.
        /// </summary>
        [BindProperty(Name = "roleId")]
        public string RoleId { get; set; }
    }
}
