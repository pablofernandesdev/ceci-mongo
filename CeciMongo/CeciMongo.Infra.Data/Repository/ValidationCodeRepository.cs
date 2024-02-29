using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Infra.CrossCutting.Settings;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace CeciMongo.Infra.Data.Repository
{
    /// <summary>
    /// Repository for managing ValidationCode entities.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ValidationCodeRepository : BaseRepository<ValidationCode>, IValidationCodeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationCodeRepository"/> class.
        /// </summary>
        /// <param name="mongoDbSettings">The MongoDB settings containing database-related information.</param>
        public ValidationCodeRepository(IOptions<MongoDbSettings> mongoDbSettings) : base(mongoDbSettings)
        {
        }
    }
}
