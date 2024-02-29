using Microsoft.AspNetCore.Mvc;

namespace CeciMongo.Domain.DTO.Role
{
    /// <summary>
    /// Data Transfer Object (DTO) representing an identifier for a role.
    /// </summary>
    public class IdentifierRoleDTO
    {
        /// <summary>
        /// Gets or sets the identifier of the role.
        /// </summary>
        [BindProperty(Name = "roleId")]
        public string RoleId { get; set; }
    }
}
