using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data.Models.PreSale;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Lite.Utilities.ReportManager
{
    public interface IPreSaleReport
    {
        public Task<FileStreamResult> CreatePreSaleReportAsync(
            CancellationToken cancellationToken,
            PreSaleGroup preSaleGroup);
    }
}