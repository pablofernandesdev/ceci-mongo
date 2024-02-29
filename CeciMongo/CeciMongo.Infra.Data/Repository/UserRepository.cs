using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CeciMongo.Infra.Data.Repository
{
    /// <summary>
    /// Class that implements the IUserRepository interface and provides methods to access user data in the database.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="mongoDbSettings">The MongoDB settings containing database-related information.</param>
        public UserRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }

        /// <summary>
        /// Retrieves a list of users based on the provided filters.
        /// </summary>
        /// <param name="filter">A UserFilterDTO object containing filter criteria.</param>
        /// <returns>A collection of User objects that match the filter criteria.</returns>
        public async Task<IEnumerable<User>> GetByFilterAsync(UserFilterDTO filter)
        {
            Expression<Func<User, bool>> query = c =>
                     (!string.IsNullOrEmpty(filter.Name) ? c.Name.Contains(filter.Name) : true) &&
                     (!string.IsNullOrEmpty(filter.Email) ? c.Email.Equals(filter.Email) : true) &&
                     (!string.IsNullOrEmpty(filter.RoleId) ? c.Role.Id.Equals(filter.RoleId) : true) &&
                     (!string.IsNullOrEmpty(filter.Search)
                         ? (c.Name.Contains(filter.Search) || c.Email.Contains(filter.Search))
                         : true);

            return await FilterByAsync(query, filter.Page, filter.PerPage);
        }

        /// <summary>
        /// Retrieves the total number of users based on the provided filters.
        /// </summary>
        /// <param name="filter">A UserFilterDTO object containing filter criteria.</param>
        /// <returns>The total number of users that match the filter criteria.</returns>
        public async Task<int> GetTotalByFilterAsync(UserFilterDTO filter)
        {
            Expression<Func<User, bool>> query = c =>
                     (!string.IsNullOrEmpty(filter.Name) ? c.Name.Contains(filter.Name) : true) &&
                     (!string.IsNullOrEmpty(filter.Email) ? c.Email.Equals(filter.Email) : true) &&
                     (!string.IsNullOrEmpty(filter.RoleId) ? c.Role.Id.Equals(filter.RoleId) : true) &&
                     (!string.IsNullOrEmpty(filter.Search)
                         ? (c.Name.Contains(filter.Search) || c.Email.Contains(filter.Search))
                         : true);

            return Convert.ToInt32(await CountByFilterAsync(query));
        }

        /// <summary>
        /// Retrieves a list of addresses for the logged-in user based on the provided filter asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the logged-in user.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>A collection of addresses that match the filter for the logged-in user.</returns>
        public async Task<IEnumerable<Address>> GetLoggedUserAddressesAsync(string userId, AddressFilterDTO filter)
        {
            var addresses = new List<Address>();

            var user = await FindByIdAsync(userId);

            if (user != null)
            {
                addresses.AddRange(user.Adresses.Where(c =>
                     (!string.IsNullOrEmpty(filter.District) ? c.District.Contains(filter.District) : true) &&
                     (!string.IsNullOrEmpty(filter.Locality) ? c.Locality.Equals(filter.Locality) : true) &&
                     (!string.IsNullOrEmpty(filter.Uf) ? c.Uf.Equals(filter.Uf) : true) &&
                     (!string.IsNullOrEmpty(filter.Search)
                         ? (c.Complement.Contains(filter.Search) || c.Street.Contains(filter.Search))
                         : true)));
            }

            return addresses
                .Skip((filter.Page - 1) * filter.PerPage)
                .Take(filter.PerPage)
                .OrderByDescending(x=> x.Id.CreationTime);
        }

        /// <summary>
        /// Retrieves the total count of addresses for the logged-in user based on the provided filter asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the logged-in user.</param>
        /// <param name="filter">The filter to apply.</param>
        /// <returns>The total count of addresses that match the filter for the logged-in user.</returns>
        public async Task<int> GetTotalLoggedUserAddressesAsync(string userId, AddressFilterDTO filter)
        {
            var addresses = new List<Address>();

            var user = await FindByIdAsync(userId);

            if (user != null)
            {
                user.Adresses.Where(c =>
                     (!string.IsNullOrEmpty(filter.District) ? c.District.Contains(filter.District) : true) &&
                     (!string.IsNullOrEmpty(filter.Locality) ? c.Locality.Equals(filter.Locality) : true) &&
                     (!string.IsNullOrEmpty(filter.Uf) ? c.Uf.Equals(filter.Uf) : true) &&
                     (!string.IsNullOrEmpty(filter.Search)
                         ? (c.Complement.Contains(filter.Search) || c.Street.Contains(filter.Search))
                         : true));
            }

            return addresses.Count;
        }
    }
}
