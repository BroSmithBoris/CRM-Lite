using System;
using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data.Dtos.PreSale;
using CRM_Lite.Data.Models.PreSale;
using Microsoft.AspNetCore.Mvc;

namespace CRM_Lite.Services
{
    public interface IPreSaleService
    {
        #region PreSale

        public Task<FileStreamResult> GetReportAsync(CancellationToken cancellationToken, Guid preSaleGroupId);

        public Task<PreSaleDto[]> GetAllAsync(CancellationToken cancellationToken);

        public Task<PreSaleStatusDto[]> GetStatusesAsync(CancellationToken cancellationToken);

        public Task<PreSaleResultDto[]> GetResultsAsync(CancellationToken cancellationToken);

        public Task<PreSaleRegionDto[]> GetRegionsAsync(CancellationToken cancellationToken);

        public Task<PreSaleDto[]> GetForTableAsync(CancellationToken cancellationToken, Guid id);

        public Task<PreSaleDto> GetAsync(CancellationToken cancellationToken, Guid id);

        public Task<PreSale> EditAsync(CancellationToken cancellationToken, Guid id, PreSaleDto preSaleDto, string requestHost);

        public Task<PreSale> CreateAsync(CancellationToken cancellationToken, PreSaleDto preSaleDto);

        public Task<PreSale> DeleteAsync(CancellationToken cancellationToken, Guid id);

        public Task<bool> ExistsAsync(CancellationToken cancellationToken, Guid id);

        #endregion

        #region PreSaleGroup

        public Task<PreSaleGroupDto[]> GetForGroupsTableAsync(CancellationToken cancellationToken);
        
        public Task<PreSaleGroupDto> GetGroupAsync(CancellationToken cancellationToken, Guid id);

        public Task<PreSaleGroupStatusDto[]> GetGroupStatusesAsync(CancellationToken cancellationToken);

        public Task<PreSaleGroup> CreateGroupAsync(CancellationToken cancellationToken, PreSaleGroupDto preSaleGroupDto);
        
        public Task EditGroupAsync(CancellationToken cancellationToken, Guid id, PreSaleGroupDto preSaleGroupDto);
        
        public Task<PreSaleGroup> DeleteGroupAsync(CancellationToken cancellationToken, Guid id);
        
        public Task<bool> GroupExistsAsync(CancellationToken cancellationToken, Guid id);

        #endregion
    }
}