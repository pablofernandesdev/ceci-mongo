using AutoMapper;
using CeciMongo.Domain.DTO.Address;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Domain.Interfaces.Service.External;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service class for managing addresses.
    /// </summary>
    public class AddressService : IAddressService
    {
        private readonly IUserRepository _userRepository;
        private readonly IViaCepService _viaCepService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddressService"/> class.
        /// </summary>
        /// <param name="userRepository">The user repository.</param>
        /// <param name="viaCepService">The ViaCep service.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public AddressService(IUserRepository userRepository,
                              IViaCepService viaCepService,
                              IMapper mapper)
        {
            _viaCepService = viaCepService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Retrieves an address by zip code asynchronously.
        /// </summary>
        /// <param name="obj">The address zip code DTO.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the address response.</returns>
        public async Task<ResultResponse<AddressResultDTO>> GetAddressByZipCodeAsync(AddressZipCodeDTO obj)
        {
            var response = new ResultResponse<AddressResultDTO>();

            try
            {
                var addressRequest = await _viaCepService.GetAddressByZipCodeAsync(obj.ZipCode);

                response.StatusCode = addressRequest.StatusCode;

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.Message = "Unable to get address. Check that the zip code was entered correctly.";
                    return response;
                }

                response.Data = _mapper.Map<AddressResultDTO>(addressRequest.Data);
            }
            catch (Exception ex)
            {
                response.Message = "Could not get address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Adds an address asynchronously.
        /// </summary>
        /// <param name="obj">The address add DTO.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the add operation response.</returns>
        public async Task<ResultResponse> AddAsync(AddressAddDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var user = await _userRepository.FindByIdAsync(obj.UserId);

                if (user is not null)
                {
                    if (obj.Main)
                    {
                        user.Adresses.ForEach(a => a.Main = false);
                    }

                    user.Adresses.Add(_mapper.Map<Address>(obj));

                    await _userRepository.ReplaceOneAsync(user);
                }

                response.Message = "Address successfully added.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not add address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Updates an address asynchronously.
        /// </summary>
        /// <param name="obj">The address update DTO.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the update operation response.</returns>
        public async Task<ResultResponse> UpdateAsync(AddressUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var user = await _userRepository.FindByIdAsync(obj.UserId);

                if (user is not null)
                {
                    var address = user.Adresses.Where(x => x.Id.Equals(obj.AddressId));

                    foreach (var item in address)
                    {
                        item.Street = obj.Street;
                        item.Uf = obj.Uf;
                        item.Number = obj.Number;
                        item.Complement = obj.Complement;
                        item.District = obj.District;
                        item.Locality = obj.Locality;
                        item.ZipCode = obj.ZipCode;
                        item.Main = obj.Main;
                    }

                    await _userRepository.ReplaceOneAsync(user);
                }

                response.Message = "Address successfully updated.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not updated address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Deletes an address asynchronously.
        /// </summary>
        /// <param name="userId">The User ID.</param>
        /// <param name="addressId">The address ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the delete operation response.</returns>
        public async Task<ResultResponse> DeleteAsync(AddressDeleteDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var user = await _userRepository.FindByIdAsync(obj.UserId);

                if (user is null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                var address = user.Adresses.FirstOrDefault(x => x.Id.Equals(MongoDB.Bson.ObjectId.Parse(obj.AddressId)));

                if (address is null)
                {
                    response.Message = "Address not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                user.Adresses.Remove(address);

                await _userRepository.ReplaceOneAsync(user);

                response.Message = "Address successfully deleted.";
            }
            catch (Exception ex)
            {
                response.Message = "Could not deleted address.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Retrieves a list of addresses asynchronously based on filter criteria.
        /// </summary>
        /// <param name="userId">The User ID.</param>
        /// <param name="filter">The address filter DTO.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the address list response.</returns>
        public async Task<ResultDataResponse<IEnumerable<AddressResultDTO>>> GetUserAddressesAsync(string userId, AddressFilterDTO filter)
        {
            var response = new ResultDataResponse<IEnumerable<AddressResultDTO>>();

            try
            {
                response.Data = _mapper.Map<IEnumerable<AddressResultDTO>>(await _userRepository.GetLoggedUserAddressesAsync(userId, filter));
                response.TotalItems = await _userRepository.GetTotalLoggedUserAddressesAsync(userId, filter);
                response.TotalPages = (int)Math.Ceiling((double)response.TotalItems / filter.PerPage);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Retrieves an address by ID asynchronously.
        /// </summary>
        /// <param name="userId">The User ID.</param>
        /// <param name="addressId">The address ID.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the address response.</returns>
        public async Task<ResultResponse<AddressResultDTO>> GetByIdAsync(AddressIdentifierDTO obj)
        {
            var response = new ResultResponse<AddressResultDTO>();

            try
            {
                var user = await _userRepository.FindByIdAsync(obj.UserId);

                if (user is null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                var address = user.Adresses.FirstOrDefault(x => x.Id.Equals(MongoDB.Bson.ObjectId.Parse(obj.AddressId)));

                if (address is null)
                {
                    response.Message = "Address not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                response.Data = _mapper.Map<AddressResultDTO>(address);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }

            return response;
        }
    }
}
