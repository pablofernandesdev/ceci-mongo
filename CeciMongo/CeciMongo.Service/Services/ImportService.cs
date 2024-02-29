using AutoMapper;
using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.Email;
using CeciMongo.Domain.DTO.Import;
using CeciMongo.Domain.DTO.Role;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Entities;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using CeciMongo.Infra.CrossCutting.Extensions;
using CeciMongo.Infra.CrossCutting.Helper;
using ClosedXML.Excel;
using Hangfire;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service responsible for importing users from a file.
    /// </summary>
    public class ImportService : IImportService
    {
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobClient _jobClient;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportService"/> class.
        /// </summary>
        /// <param name="emailService">An instance of the <see cref="IEmailService"/> used for email-related operations.</param>
        /// <param name="mapper">An instance of the <see cref="IMapper"/> used for object mapping.</param>
        /// <param name="jobClient">An instance of the <see cref="IBackgroundJobClient"/> used for managing background jobs.</param>
        /// <param name="userRepository">An instance of the <see cref="IUserRepository"/> used for user-related database operations.</param>
        /// <param name="roleRepository">An instance of the <see cref="IRoleRepository"/> used for role-related database operations.</param>
        public ImportService(IEmailService emailService,
            IMapper mapper,
            IBackgroundJobClient jobClient,
            IUserRepository userRepository,
            IRoleRepository roleRepository)
        {
            _emailService = emailService;
            _mapper = mapper;
            _jobClient = jobClient;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
        }

        /// <summary>
        /// Imports users from a file.
        /// </summary>
        /// <param name="model">The file upload information.</param>
        /// <returns>A response indicating the success of the import operation.</returns>
        public async Task<ResultResponse> ImportUsersAsync(FileUploadDTO model)
        {
            var response = new ResultResponse();

            try
            {
                var filePath = await SaveFileAsync(model);

                if (Path.GetExtension(filePath).Equals(".csv"))
                {
                    filePath = ConvertToExcelAsync(filePath);
                }

                var users = await ReadUsersFromExcelAsync(filePath);

                await _userRepository.InsertManyAsync(_mapper.Map<IEnumerable<User>>(users));

                await SendEmailsToUsersAsync(users);

                DeleteFile(filePath);

                response.Message = "Users imported successfully.";
            }
            catch (Exception ex)
            {
                response.Message = "Unable to import users.";
                response.Exception = ex;
            }

            return response;
        }

        // Helper method to save the file to disk
        private async Task<string> SaveFileAsync(FileUploadDTO model)
        {
            var fileName = Path.GetFileName(Guid.NewGuid().ToString() + model.File.FileName);
            //var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\UploadFiles", fileName);
            var filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net6.0", string.Empty)),
                @"wwwroot\UploadFiles",
                fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(fileStream);
            }

            return filePath;
        }

        // Helper method to convert CSV file to Excel format
        private static string ConvertToExcelAsync(string atualFile)
        {
            //var newFile = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\UploadFiles", Guid.NewGuid().ToString() + ".xlsx");
            var newFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.Replace("bin\\Debug\\net6.0", string.Empty)),
                @"wwwroot\UploadFiles",
                Guid.NewGuid().ToString() + ".xlsx");

            var csvLines = File.ReadAllLines(atualFile, Encoding.UTF8).Select(a => a.Split(';'));

            int rowCount = 0;
            int colCount = 0;

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add();

                rowCount = 1;

                foreach (var line in csvLines)
                {
                    colCount = 1;
                    foreach (var col in line)
                    {
                        worksheet.Cell(rowCount, colCount).Value = col;
                        colCount++;
                    }
                    rowCount++;
                }

                File.Delete(atualFile);

                workbook.SaveAs(newFile);
            }

            return newFile;
        }

        // Helper method to read users from Excel file
        private async Task<List<UserImportDTO>> ReadUsersFromExcelAsync(string filePath)
        {
            var users = new List<UserImportDTO>();

            var basicRole = await _roleRepository.GetBasicProfile();

            using (var excelWorkbook = new XLWorkbook(filePath))
            {
                var nonEmptyDataRows = excelWorkbook.Worksheets.FirstOrDefault().RowsUsed().Skip(1);

                foreach (var dataRow in nonEmptyDataRows)
                {
                    var cellName = dataRow.Cell(1).Value;
                    var cellEmail = dataRow.Cell(2).Value;

                    var password = PasswordExtension.GeneratePassword(2, 2, 2, 2);

                    users.Add(new UserImportDTO
                    {
                        Name = cellName.ToString(),
                        Email = cellEmail.ToString(),
                        Role = _mapper.Map<RoleResultDTO>(basicRole),
                        Password = PasswordExtension.EncryptPassword(StringHelper.Base64Encode(password)),
                        PasswordBase64Decode = password
                    });
                }
            }

            return users;
        }

        // Helper method to send emails to users
        private async Task SendEmailsToUsersAsync(List<UserImportDTO> users)
        {
            foreach (var item in users)
            {
                _jobClient.Enqueue(() => _emailService.SendEmailAsync(new EmailRequestDTO
                {
                    Body = "Your registration was carried out in the application. Use the password <b>" + item.PasswordBase64Decode + "</b> on your first access to the application.",
                    Subject = item.Name,
                    ToEmail = item.Email
                }));
            }
        }

        // Helper method to delete the file
        private void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }
    }
}
