using AutoMapper;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service class for managing roles.
    /// </summary>
    public class RoleService : IRoleService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<RoleService> _logger;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleService"/> class.
        /// </summary>
        /// <param name="mapper">An instance of AutoMapper's IMapper used for object mapping.</param>
        /// <param name="logger">An instance of ILogger<T> used for logging within the RoleService.</param>
        /// <param name="roleRepository">An implementation of the IRoleRepository interface, providing access to role-related data storage and retrieval.</param>
        public RoleService(
            IMapper mapper, 
            ILogger<RoleService> logger,
            IRoleRepository roleRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Retrieves all roles.
        /// </summary>
        /// <returns>A response containing the collection of roles.</returns>
        public async Task<ResultDataResponse<IEnumerable<RoleResultDTO>>> GetAsync()
        {
            var response = new ResultDataResponse<IEnumerable<RoleResultDTO>>();

            try
            {
                response.Data = _mapper.Map<IEnumerable<RoleResultDTO>>(await _roleRepository.FilterByAsync(x => x.Active));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Adds a new role.
        /// </summary>
        /// <param name="obj">The role object to add.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        public async Task<ResultResponse> AddAsync(RoleAddDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                await _roleRepository.InsertOneAsync(_mapper.Map<Role>(obj));

                response.Message = "Role successfully added.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding role");
                response.Message = "Error adding role.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Deletes a role by ID.
        /// </summary>
        /// <param name="id">The ID of the role to delete.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        public async Task<ResultResponse> DeleteAsync(string id)
        {
            var response = new ResultResponse();

            try
            {
                var role = await _roleRepository.FindByIdAsync(id);

                if (role is null)
                {
                    response.Message = "Role not found";
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return response;
                }

                await _roleRepository.DeleteByIdAsync(id);

                response.Message = "Role successfully delete.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role");
                response.Message = "Error deleting role.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Updates a role.
        /// </summary>
        /// <param name="obj">The updated role object.</param>
        /// <returns>A response indicating the result of the operation.</returns>
        public async Task<ResultResponse> UpdateAsync(RoleUpdateDTO obj)
        {
            var response = new ResultResponse();

            try
            {
                var role = await _roleRepository.FindByIdAsync(obj.RoleId);

                if (role is null)
                {
                    response.Message = "Role not found";
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    return response;
                }

                role = _mapper.Map(obj, role);

                await _roleRepository.ReplaceOneAsync(role);

                response.Message = "Role successfully update.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not update role.");
                response.Message = "Could not update role.";
                response.Exception = ex;
            }

            return response;
        }

        /// <summary>
        /// Retrieves a role by ID.
        /// </summary>
        /// <param name="id">The ID of the role to retrieve.</param>
        /// <returns>A response containing the role.</returns>
        public async Task<ResultResponse<RoleResultDTO>> GetByIdAsync(string id)
        {
            var response = new ResultResponse<RoleResultDTO>();

            try
            {
                var role = await _roleRepository.FindByIdAsync(id);

                if (role is null)
                {
                    response.Message = "Função não encontrada.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                response.Data = _mapper.Map<RoleResultDTO>(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role");
                response.Message = "It was not possible to search the role.";
                response.Exception = ex;
            }

            return response;
        }
    }
}
