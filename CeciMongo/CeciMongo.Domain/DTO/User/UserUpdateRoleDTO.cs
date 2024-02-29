namespace CeciMongo.Domain.DTO.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing user information for updating user role.
    /// </summary>
    public class UserUpdateRoleDTO
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user whose role is to be updated.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the new role identifier to be assigned to the user.
        /// </summary>
        public string RoleId { get; set; }
    }
}
