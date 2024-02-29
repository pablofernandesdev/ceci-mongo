using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.ViaCep;
using System.Threading.Tasks;

namespace CeciMongo.Domain.Interfaces.Service.External
{
    /// <summary>
    /// Represents a service interface for interacting with the ViaCep API to retrieve address information by zip code.
    /// </summary>
    public interface IViaCepService
    {
        /// <summary>
        /// Retrieves address information asynchronously based on a specified zip code.
        /// </summary>
        /// <param name="zipCode">The zip code to retrieve address information for.</param>
        /// <returns>A task representing the asynchronous operation. A ResultResponse containing the retrieved address information from ViaCep.</returns>
        Task<ResultResponse<ViaCepAddressResponseDTO>> GetAddressByZipCodeAsync(string zipCode);
    }
}
