using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CeciMongo.Domain.Interfaces.Repository
{
    /// <summary>
    /// Represents a repository interface for managing User entities.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Retrieves a collection of user entities based on the provided filter asynchronously.
        /// </summary>
        /// <param name="filter">The filter criteria for retrieving user entities.</param>
        /// <returns>A task representing the asynchronous operation. A collection of user entities matching the filter.</returns>
        Task<IEnumerable<User>> GetByFilterAsync(UserFilterDTO filter);

        /// <summary>
        /// Retrieves the total count of user entities based on the provided filter asynchronously.
        /// </summary>
        /// <param name="filter">The filter criteria for counting user entities.</param>
        /// <returns>A task representing the asynchronous operation. The total count of user entities matching the filter.</returns>
        Task<int> GetTotalByFilterAsync(UserFilterDTO filter);

        Task<IEnumerable<Address>> GetLoggedUserAddressesAsync(string userId, AddressFilterDTO filter);

        Task<int> GetTotalLoggedUserAddressesAsync(string userId, AddressFilterDTO filter);
    }
}
