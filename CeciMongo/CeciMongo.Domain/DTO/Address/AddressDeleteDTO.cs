using Microsoft.AspNetCore.Mvc;

namespace CeciMongo.Domain.DTO.Address
{
    /// <summary>
    /// Data Transfer Object (DTO) representing the information for deleting an address.
    /// </summary>
    public class AddressDeleteDTO
    {
        /// <summary>
        /// Gets or sets the identifier of the user associated with the address to be deleted.
        /// </summary>
        /// <remarks>
        /// This property is bound to the "userId" parameter in the HTTP request using the [BindProperty] attribute.
        /// </remarks>
        [BindProperty(Name = "userId")]
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the address to be deleted.
        /// </summary>
        /// <remarks>
        /// This property is bound to the "addressId" parameter in the HTTP request using the [BindProperty] attribute.
        /// </remarks>
        [BindProperty(Name = "addressId")]
        public string AddressId { get; set; }
    }
}
