using System;
using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data.Dtos;

namespace CRM_Lite.Services
{
    public interface IDepartmentService
    {
        Task<DepartmentDto[]> GetDepartments(CancellationToken cancellationToken);
    }
}