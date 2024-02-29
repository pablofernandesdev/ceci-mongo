﻿using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Test.Fakers.Commons;
using CeciMongo.Test.Fakers.Role;
using CeciMongo.WebApplication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace CeciMongo.Test.Controllers
{
    public class RoleControllerTest
    {
        private readonly Moq.Mock<IRoleService> _mockRoleService;

        public RoleControllerTest()
        {
            _mockRoleService = new Moq.Mock<IRoleService>();
        }

        [Fact]
        public async Task Add_role()
        {
            //Arrange
            var roleAddDTO = RoleFaker.RoleAddDTO().Generate();
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockRoleService.Setup(x => x.AddAsync(roleAddDTO))
                .ReturnsAsync(resultResponse);

            var roleController = RoleControllerConstrutor();

            //Act
            var result = await roleController.Add(roleAddDTO);
            _mockRoleService.Verify(x => x.AddAsync(roleAddDTO), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Update_role()
        {
            //Arrange
            var roleUpdateDTO = RoleFaker.RoleUpdateDTO().Generate();
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockRoleService.Setup(x => x.UpdateAsync(roleUpdateDTO))
                .ReturnsAsync(resultResponse);

            var roleController = RoleControllerConstrutor();

            //Act
            var result = await roleController.Update(roleUpdateDTO);
            _mockRoleService.Verify(x => x.UpdateAsync(roleUpdateDTO), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Delete_role()
        {
            //Arrange
            var resultResponse = ResultResponseFaker.ResultResponse(It.IsAny<HttpStatusCode>());

            _mockRoleService.Setup(x => x.DeleteAsync(It.IsAny<string>()))
                .ReturnsAsync(resultResponse);

            var roleController = RoleControllerConstrutor();

            //Act
            var result = await roleController.Delete(new Domain.DTO.Role.RoleDeleteDTO { 
                RoleId = It.IsAny<string>()
            });
            _mockRoleService.Verify(x => x.DeleteAsync(It.IsAny<string>()), Times.Once);

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse>(objResult.Value);
        }

        [Fact]
        public async Task Get_all()
        {
            //Arrange
            var roleResultDTO = RoleFaker.RoleResultDTO().Generate(3);
            var resultDataResponse = ResultDataResponseFaker.ResultDataResponse<IEnumerable<RoleResultDTO>>(roleResultDTO, It.IsAny<HttpStatusCode>());

            _mockRoleService.Setup(x => x.GetAsync())
                .ReturnsAsync(resultDataResponse);

            var userController = RoleControllerConstrutor();

            //Act
            var result = await userController.Get();

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultDataResponse<IEnumerable<RoleResultDTO>>>(objResult.Value);
        }

        [Fact]
        public async Task Get_role_by_id()
        {
            //Arrange
            var roleResultDTO = RoleFaker.RoleResultDTO().Generate();
            var resultDataResponse = ResultResponseFaker.ResultResponseData(roleResultDTO, It.IsAny<HttpStatusCode>());

            _mockRoleService.Setup(x => x.GetByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(resultDataResponse);

            var roleController = RoleControllerConstrutor();

            //Act
            var result = await roleController.GetById(new IdentifierRoleDTO { RoleId = It.IsAny<string>() } );

            //Assert
            var objResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.IsType<ResultResponse<RoleResultDTO>>(objResult.Value);
        }

        private RoleController RoleControllerConstrutor()
        {
            return new RoleController(_mockRoleService.Object);
        }
    }
}
