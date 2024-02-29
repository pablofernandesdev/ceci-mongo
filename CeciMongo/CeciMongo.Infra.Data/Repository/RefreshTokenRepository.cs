using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace CeciMongo.Infra.Data.Repository
{
    /// <summary>
    /// Repository for managing RefreshToken entities.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshTokenRepository"/> class.
        /// </summary>
        /// <param name="mongoDbSettings">The MongoDB settings containing database-related information.</param>
        public RefreshTokenRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }
    }
}
