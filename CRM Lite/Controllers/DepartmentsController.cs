using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data;
using CRM_Lite.Data.Models;
using CRM_Lite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM_Lite.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(ApplicationContext context, IDepartmentService departmentService)
        {
            _context = context;
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<Department[]>> GetDepartmentsAsync()
        {
            return await _context.Departments.ToArrayAsync();
        }

        [HttpGet("MainDepartments")]
        public async Task<IActionResult> MainDepartments(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _departmentService.GetDepartments(cancellationToken))
            );
        }
    }
}