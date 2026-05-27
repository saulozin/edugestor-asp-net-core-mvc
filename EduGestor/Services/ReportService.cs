using EduGestor.Data;
using EduGestor.Models.Enums;
using EduGestor.Services.Exceptions;
using EduGestor.ViewModels;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EduGestor.Services
{
    public class ReportService
    {
        private readonly EduGestorContext _context;

        private readonly AcademicRulesService _academicRulesService;

        public ReportService(EduGestorContext context, AcademicRulesService academicRulesService)
        {
            _context = context;

            _academicRulesService = academicRulesService;
        }

        public async Task<byte[]> GenerateReportCardAsync(
            Guid registrationId)
        {
            var registration =
                await _context.Registrations
                    .Include(r => r.Student)
                        .ThenInclude(s => s.Guardian)
                    .Include(r => r.StudentClass)
                    .Include(r => r.Grades)
                        .ThenInclude(g => g.DisciplineClass)
                            .ThenInclude(dc => dc.Discipline)
                    .FirstOrDefaultAsync(r =>
                        r.Id == registrationId);

            if (registration == null)
            {
                throw new NotFoundException("Registration not found.");
            }

            var disciplines =
                registration.Grades
                    .GroupBy(g => new
                    {
                        g.DisciplineClassId,

                        Discipline = g.DisciplineClass!.Discipline!.Name
                    })

                    .Select(g =>
                    {
                        var disciplineClassId =
                            g.Key.DisciplineClassId ?? Guid.Empty;

                        var average =
                            g.Average(x => x.StudentGrade);

                        var frequency =
                            registration.GetAttendance(disciplineClassId);

                        var status =
                            _academicRulesService.CalculateStatus(average, frequency);

                        return new ReportCardDisciplineViewModel
                        {
                            Discipline = g.Key.Discipline,

                            Average = average,

                            Frequency = frequency,

                            Status = GetStatusLabel(status)
                        };
                    })

                    .ToList();

            var averageGrade =
                disciplines.Any()
                    ? disciplines.Average(d => d.Average) : 0;

            var averageFrequency =
                disciplines.Any()
                    ? disciplines.Average(d => d.Frequency) : 0;

            var approved =
                disciplines.All(d =>
                    d.Status == AcademicStatus.Approved.ToString());

            var model =
                new ReportCardViewModel
                {
                    StudentName =
                        registration.Student?.Name ?? "",

                    GuardianName =
                        registration.Student?
                            .Guardian?.Name ?? "",

                    ClassName =
                        registration.StudentClass?
                            .Code ?? "",

                    SchoolYear =
                        DateTime.Now.Year,

                    AverageGrade =
                        averageGrade,

                    AverageFrequency =
                        averageFrequency,

                    Approved =
                        approved,

                    Disciplines =
                        disciplines
                };

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Size(PageSizes.A4);

                    page.DefaultTextStyle(x =>
                        x.FontSize(12));

                    // HEADER
                    page.Header()
                        .Text("Boletim Escolar")
                        .SemiBold()
                        .FontSize(24)
                        .FontColor(Colors.Blue.Medium);

                    // CONTENT
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            // STUDENT INFO
                            column.Item().Text(
                                $"Estudante: {model.StudentName}");

                            column.Item().Text(
                                $"Responsável: {model.GuardianName}");

                            column.Item().Text(
                                $"Turma: {model.ClassName}");

                            column.Item().Text(
                                $"Ano Letivo: {model.SchoolYear}");

                            // TABLE
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);

                                    columns.RelativeColumn();

                                    columns.RelativeColumn();

                                    columns.RelativeColumn();
                                });

                                // HEADER
                                table.Header(header =>
                                {
                                    header.Cell()
                                        .Element(CellStyle)
                                        .Text("Disciplina");

                                    header.Cell()
                                        .Element(CellStyle)
                                        .Text("Média");

                                    header.Cell()
                                        .Element(CellStyle)
                                        .Text("Frequência");

                                    header.Cell()
                                        .Element(CellStyle)
                                        .Text("Situação");
                                });

                                // ROWS
                                foreach (var item in model.Disciplines)
                                {
                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(item.Discipline);

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(
                                            item.Average.ToString("F1"));

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(
                                            $"{item.Frequency:F0}%");

                                    table.Cell()
                                        .Element(CellStyle)
                                        .Text(item.Status);
                                }
                            });

                            // FINAL RESULT
                            column.Item()
                                .PaddingTop(20)
                                .Text(
                                    $"Resultado Final: " +
                                    $"{(model.Approved
                                        ? "APROVADO"
                                        : "REPROVADO")}")
                                .Bold()
                                .FontSize(16);
                        });

                    // FOOTER
                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("EduGestor - ");
                            x.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf();

            static IContainer CellStyle(
                IContainer container)
            {
                return container
                    .Border(1)
                    .BorderColor(Colors.Grey.Lighten2)
                    .Padding(5);
            }
        }

        private string GetStatusLabel(AcademicStatus status)
        {
            return status switch
            {
                AcademicStatus.Approved =>
                    "Aprovado",

                AcademicStatus.Recovery =>
                    "Recuperação",

                AcademicStatus.FailedByAttendance =>
                    "Reprovado por frequência",

                AcademicStatus.FailedByGrade =>
                    "Reprovado por nota",

                _ =>
                    "In Progress"
            };
        }
    }

}
