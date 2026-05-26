using EduGestor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduGestor.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ReportService _reportService;

        public ReportsController(
            ReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> ReportCard(
            Guid registrationId)
        {
            var pdf =
                await _reportService
                    .GenerateReportCardAsync(
                        registrationId);

            return File(
                pdf,
                "application/pdf",
                "boletim-escolar.pdf");
        }
    }
}
