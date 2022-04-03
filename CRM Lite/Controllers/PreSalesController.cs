using System;
using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data.Dtos.PreSale;
using CRM_Lite.Services;
using Microsoft.AspNetCore.Mvc;
using Vostok.Logging.Abstractions;

namespace CRM_Lite.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class PreSalesController : ControllerBase
    {
        private readonly ILog _log;
        private readonly IPreSaleService _preSaleService;

        public PreSalesController
        (
            ILog log,
            IPreSaleService preSaleService)
        {
            _log = log;
            _preSaleService = preSaleService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.IsTop = true;
            ViewBag.IsCanRead = true;
            ViewBag.IsCanEdit = true;

            return View();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> PreSale(Guid id)
        {
            ViewBag.IsTop = true;
            ViewBag.IsMarketing = true;

            ViewBag.IsCanRead = true;
            ViewBag.IsCanEdit = true;

            return View();
        }

        [HttpGet("PreSaleStatuses")]
        public async Task<IActionResult> GetPreSaleStatusesAsync(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetStatusesAsync(cancellationToken))
            );
        }

        #region Pre-sale

        [HttpGet("GetPreSaleRept/{preSaleGroupId:guid}")]
        public async Task<IActionResult> GetPreSaleReptAsync(CancellationToken cancellationToken, Guid preSaleGroupId)
        {
            return await SecuredRunAsync(
                async () =>
                    await _preSaleService.GetReportAsync(cancellationToken, preSaleGroupId)
            );
        }

        [HttpGet("AllPreSales")]
        public async Task<IActionResult> AllPreSales(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetAllAsync(cancellationToken))
            );
        }

        [HttpGet("PreSaleResults")]
        public async Task<IActionResult> GetPreSaleResultsAsync(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetResultsAsync(cancellationToken))
            );
        }

        [HttpGet("PreSaleRegions")]
        public async Task<IActionResult> GetPreSaleRegionsAsync(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetRegionsAsync(cancellationToken))
            );
        }

        [HttpGet("ForPreSalesTable/{id}")]
        public async Task<IActionResult> GetForPreSalesTableAsync(CancellationToken cancellationToken,
            [FromRoute] Guid id)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetForTableAsync(cancellationToken, id))
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPreSaleAsync(CancellationToken cancellationToken,
            [FromRoute] Guid id)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetAsync(cancellationToken, id))
            );
        }

        [HttpPut("EditPreSale/{id}")]
        public async Task<IActionResult> EditPreSaleAsync(CancellationToken cancellationToken, [FromRoute] Guid id,
            [FromBody] PreSaleDto preSaleDto)
        {
            return await SecuredRunAsync(
                async () => Json(await _preSaleService.EditAsync(cancellationToken, id, preSaleDto, Request.Host.Value)));
        }

        [HttpPost("CreatePreSale")]
        public async Task<IActionResult> CreatePreSaleAsync(CancellationToken cancellationToken,
            [FromBody] PreSaleDto preSaleDto)
        {
            return await SecuredRunAsync(
                async () =>
                {
                    var preSale = await _preSaleService.CreateAsync(cancellationToken, preSaleDto);
                    return CreatedAtAction("CreatePreSale", new {id = preSale.Id}, preSale);
                }
            );
        }

        [HttpDelete("DeletePreSale/{id}")]
        public async Task<IActionResult> DeletePreSaleAsync(CancellationToken cancellationToken, [FromRoute] Guid id)
        {
            return await SecuredRunAsync(
                async () => Json(await _preSaleService.DeleteAsync(cancellationToken, id)));
        }

        [HttpGet("PreSaleExists")]
        public async Task<bool> PreSaleExists(CancellationToken cancellationToken, Guid id)
        {
            return await _preSaleService.ExistsAsync(cancellationToken, id);
        }

        #endregion


        #region Pre-sale group

        [HttpGet("PreSaleGroup/{id}")]
        public async Task<IActionResult> GetPreSaleGroupAsync(CancellationToken cancellationToken,
            [FromRoute] Guid id)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetGroupAsync(cancellationToken, id))
            );
        }

        [HttpGet("ForGroupsTable")]
        public async Task<IActionResult> GetForGroupsTableAsync(CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetForGroupsTableAsync(cancellationToken)));
        }

        [HttpGet("PreSaleGroupStatuses")]
        public async Task<IActionResult> GetPreSaleGroupStatusesAsync(
            CancellationToken cancellationToken)
        {
            return await SecuredRunAsync(
                async () =>
                    Json(await _preSaleService.GetGroupStatusesAsync(cancellationToken))
            );
        }

        [HttpPost("CreatePreSaleGroup")]
        public async Task<IActionResult> CreatePreSaleGroupAsync(CancellationToken cancellationToken,
            [FromBody] PreSaleGroupDto preSaleGroupDto)
        {
            return await SecuredRunAsync(
                async () =>
                {
                    var preSaleGroup = await _preSaleService.CreateGroupAsync(cancellationToken, preSaleGroupDto);
                    return CreatedAtAction("CreatePreSaleGroup", new {id = preSaleGroup.Id}, preSaleGroup);
                }
            );
        }

        [HttpPut("EditPreSaleGroup/{id}")]
        public async Task<IActionResult> EditPreSaleGroupAsync(CancellationToken cancellationToken,
            [FromRoute] Guid id, [FromBody] PreSaleGroupDto preSaleGroupDto)
        {
            return await SecuredRunAsync(
                async () =>
                {
                    await _preSaleService.EditGroupAsync(cancellationToken, id, preSaleGroupDto);

                    return NoContent();
                }
            );
        }

        [HttpDelete("DeletePreSaleGroup/{id}")]
        public async Task<IActionResult> DeletePreSaleGroupAsync(CancellationToken cancellationToken,
            [FromRoute] Guid id)
        {
            return await SecuredRunAsync(
                async () => Json(await _preSaleService.DeleteGroupAsync(cancellationToken, id)));
        }

        [HttpGet("PreSaleGroupExists/{id:guid}")]
        public async Task<bool> PreSaleGroupExists(CancellationToken cancellationToken, Guid id)
        {
            return await _preSaleService.GroupExistsAsync(cancellationToken, id);
        }

        #endregion
    }
}