﻿using Microsoft.AspNetCore.Mvc;

namespace CeciMongo.Domain.DTO.User
{
    /// <summary>
    /// Data Transfer Object (DTO) representing the deletion of a user.
    /// </summary>
    public class UserDeleteDTO
    {
        /// <summary>
        /// Gets or sets the identifier of the user to be deleted.
        /// </summary>
        [BindProperty(Name = "userId")]
        public string UserId { get; set; }
    }
}
