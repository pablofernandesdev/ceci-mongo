using CeciMongo.Domain.DTO.Commons;
using CeciMongo.Domain.DTO.User;
using CeciMongo.Domain.Interfaces.Repository;
using CeciMongo.Domain.Interfaces.Service;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CeciMongo.Service.Services
{
    /// <summary>
    /// Service class for managing report.
    /// </summary>
    public class ReportService : IReportService
    {
        private readonly ILogger<ReportService> _logger;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegisterService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="userRepository">An instance of the <see cref="IUserRepository"/> used for user-related database operations.</param>
        public ReportService(ILogger<ReportService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Generates a report with user data in Excel format.
        /// </summary>
        /// <param name="filter">Filter criteria for selecting users.</param>
        /// <returns>Response containing the generated report as a byte array.</returns>
        public async Task <ResultResponse<byte[]>> GenerateUsersReport(UserFilterDTO filter)
        {
            var response = new ResultResponse<byte[]>();

            try
            {
                var users = await _userRepository.GetByFilterAsync(filter);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Users");

                    worksheet.Cell("A1").Style.Font.SetBold();
                    worksheet.Cell("B1").Style.Font.SetBold();
                    worksheet.Cell("C1").Style.Font.SetBold();

                    worksheet.Cell("A1").Value = "ID";
                    worksheet.Cell("B1").Value = "Name";
                    worksheet.Cell("C1").Value = "Email";

                    var currentRow = 2;

                    if (users.Any())
                    {
                        foreach (var item in users)
                        {
                            worksheet.Cell("A" + currentRow).Value = item.Id.ToString();
                            worksheet.Cell("B" + currentRow).Value = item.Name;
                            worksheet.Cell("C" + currentRow).Value = item.Email;

                            currentRow++;
                        }
                    }

                    using (var ms = new MemoryStream())
                    {
                        workbook.SaveAs(ms);
                        response.Data = ms.ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                response.Message = "Unable to generate report.";
                response.Exception = ex;
            }

            return response;
        }
    }
}
