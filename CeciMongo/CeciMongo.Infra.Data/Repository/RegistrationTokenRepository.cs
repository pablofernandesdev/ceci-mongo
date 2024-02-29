using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace CeciMongo.Infra.Data.Repository
{
    /// <summary>
    /// Repository for managing RegistrationToken entities.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RegistrationTokenRepository : BaseRepository<RegistrationToken>, IRegistrationTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationTokenRepository"/> class.
        /// </summary>
        /// <param name="mongoDbSettings">The MongoDB settings containing database-related information.</param>
        public RegistrationTokenRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }
    }
}
