using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CeciMongo.Infra.Data.Repository
{
    /// <summary>
    /// Repository for managing Role entities.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        private readonly RoleSettings _roleSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleRepository"/> class.
        /// </summary>
        /// <param name="roleSettings">The profile settings containing profile-related information.</param>
        /// <param name="mongoDbSettings">The MongoDB settings containing database-related information.</param>
        public RoleRepository(IOptions<RoleSettings> roleSettings, IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
            _roleSettings = roleSettings.Value;
        }

        /// <summary>
        /// Retrieves the basic profile role.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. The task result contains the basic profile role.</returns>
        public async Task<Role> GetBasicProfile()
        {
            var basicProfile = await FindOneAsync(x=> x.Name.Equals(_roleSettings.BasicRoleName));

            if (basicProfile == null)
            {
                throw new System.Exception("Basic profile not configured.");
            }

            return basicProfile;
        }
    }
}
