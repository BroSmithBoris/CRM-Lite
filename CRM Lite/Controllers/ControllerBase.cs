using System;
using System.Threading.Tasks;
using CRM.API.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Lite.Controllers
{
    public class ControllerBase : Controller
    {
        protected async Task<IActionResult> SecuredRunAsync(Func<Task<IActionResult>> func)
        {
            try
            {
                return await func();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (NoAccessException)
            {
                return Forbid();
            }
            catch (BadRequestException exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}