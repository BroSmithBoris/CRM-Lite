using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM_Lite.Data;
using CRM_Lite.Data.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CRM_Lite.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;

        public DepartmentService(ApplicationContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<DepartmentDto[]> GetDepartments(CancellationToken cancellationToken)
        {
            var departments = await _context.Departments
                .Where(e => e.ParentDepartment == null).ToArrayAsync(cancellationToken);

            var departmentsDto = _mapper.Map<DepartmentDto[]>(departments);

            return departmentsDto;
        }
    }
}