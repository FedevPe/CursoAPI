using MasterAPI.Application.Interfaces;
using MasterAPI.Domain.Models;
using MasterAPI.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace MasterAPI.Application.Cursos.CursoReport.csv
{
    public class CursoReportCsvQuery
    {
        public record CursoReportQueryRequest : IRequest<MemoryStream>;

        internal class CursoReportCsvQueryHandle(
            MasterAPIDbContext context,
            IReportService<Curso> reportService
        ):  IRequestHandler<CursoReportQueryRequest, MemoryStream>
        {
            public async Task<MemoryStream> Handle(
                CursoReportQueryRequest request, 
                CancellationToken cancellationToken)
            {
                var cursos = await context.Cursos!.Take(10).Skip(0).ToListAsync();
                return await reportService.GetCsvReport(cursos);
            }
        }
    }
}