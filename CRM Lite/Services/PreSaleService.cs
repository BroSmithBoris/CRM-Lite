using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CRM.API.Exceptions;
using CRM_Lite.Data;
using CRM_Lite.Data.Dtos.PreSale;
using CRM_Lite.Data.Models.PreSale;
using CRM_Lite.Utilities.ReportManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vostok.Logging.Abstractions;

namespace CRM_Lite.Services
{
    public class PreSaleService : IPreSaleService
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly ILog _log;
        private readonly IPreSaleReport _preSaleReport;

        public PreSaleService(
            ApplicationContext context,
            IMapper mapper,
            ILog log,
            IPreSaleReport preSaleReport)
        {
            _context = context;
            _mapper = mapper;
            _log = log;
            _preSaleReport = preSaleReport;
        }

        #region PreSale

        public async Task<FileStreamResult> GetReportAsync(CancellationToken cancellationToken, Guid preSaleGroupId)
        {
            var preSaleGroup = await _context.PreSaleGroups
                .SingleOrDefaultAsync(psg => psg.Id == preSaleGroupId, cancellationToken);

            if (preSaleGroup == null)
            {
                throw new NotFoundException();
            }

            return await _preSaleReport.CreatePreSaleReportAsync(cancellationToken, preSaleGroup);
        }

        public async Task<PreSaleDto[]> GetAllAsync(CancellationToken cancellationToken)
        {
            var preSales = await _context.PreSales
                .Include(ps => ps.ResponsibleUser)
                .Include(ps => ps.Status)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<PreSaleDto[]>(preSales);
        }

        public async Task<PreSaleStatusDto[]> GetStatusesAsync(CancellationToken cancellationToken)
        {
            var preSaleStatuses = await _context.PreSaleStatuses
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<PreSaleStatusDto[]>(preSaleStatuses);
        }

        public async Task<PreSaleResultDto[]> GetResultsAsync(CancellationToken cancellationToken)
        {
            var preSaleResults = await _context.PreSaleResults
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<PreSaleResultDto[]>(preSaleResults);
        }

        public async Task<PreSaleRegionDto[]> GetRegionsAsync(CancellationToken cancellationToken)
        {
            var preSaleRegions = await _context.PreSaleRegions
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<PreSaleRegionDto[]>(preSaleRegions);
        }

        public async Task<PreSaleDto[]> GetForTableAsync(CancellationToken cancellationToken, Guid id)
        {
            var preSalesInGroup = await _context.PreSales
                .Include(ps => ps.Status)
                .Include(ps => ps.Result)
                .Include(ps => ps.Region)
                .Include(ps => ps.ResponsibleUser)
                .Include(ps => ps.Group)
                .Where(ps => ps.Group.Id == id)
                .ToArrayAsync(cancellationToken);

            if (preSalesInGroup == null)
            {
                throw new NotFoundException();
            }

            var preSalesInGroupDto = _mapper.Map<PreSaleDto[]>(preSalesInGroup);

            return preSalesInGroupDto;
        }

        public async Task<PreSaleDto> GetAsync(CancellationToken cancellationToken, Guid id)
        {
            var preSale = await _context.PreSales
                .SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

            if (preSale == null)
            {
                throw new NotFoundException();
            }

            var preSaleDto = _mapper.Map<PreSaleDto>(preSale);

            return preSaleDto;
        }

        public async Task<PreSale> EditAsync(CancellationToken cancellationToken, Guid id, PreSaleDto preSaleDto, string requestHost)
        {
            _log.Info("Pre-sale start Edit");

            if (id != preSaleDto.Id)
            {
                throw new BadRequestException();
            }
            

            var preSale = _mapper.Map<PreSale>(preSaleDto);
            
            preSale.ChangedDate = DateTime.UtcNow;

            _context.Entry(preSale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(cancellationToken, id))
                {
                    throw new NotFoundException();
                }

                throw;
            }

            return preSale;
        }

        public async Task<PreSale> CreateAsync(CancellationToken cancellationToken, PreSaleDto preSaleDto)
        {
            _log.Info("Pre-sale start Create");

            var preSale = _mapper.Map<PreSale>(preSaleDto);

            preSale.CreatedDate = DateTime.UtcNow;
            preSale.ChangedDate = preSale.CreatedDate;

            await _context.PreSales.AddAsync(preSale, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            _log.Info(
                $"Pre-sale {preSale.Organization} ({preSale.Id}) in {preSale.GroupId} was created");

            return preSale;
        }

        public async Task<PreSale> DeleteAsync(CancellationToken cancellationToken, Guid id)
        {
            _log.Info("Pre-sale start Delete");

            var preSale = await _context.PreSales
                .Include(ps => ps.Status)
                .Include(ps => ps.Result)
                .Include(ps => ps.Region)
                .Include(ps => ps.ResponsibleUser)
                .Include(ps => ps.Group)
                .SingleOrDefaultAsync(psg => psg.Id == id, cancellationToken: cancellationToken);

            if (preSale == null)
            {
                throw new NotFoundException();
            }


            _context.PreSales.Remove(preSale);
            await _context.SaveChangesAsync(cancellationToken);

            return preSale;
        }

        public async Task<bool> ExistsAsync(CancellationToken cancellationToken, Guid id) =>
            await _context.PreSales.AnyAsync(ps => ps.Id == id, cancellationToken);

        #endregion

        #region PreSaleGroup

        public async Task<PreSaleGroupDto[]> GetForGroupsTableAsync(CancellationToken cancellationToken)
        {
            var preSaleGroups = await _context.PreSaleGroups
                .Include(psg => psg.Status)
                .Include(psg => psg.Department)
                .ToArrayAsync(cancellationToken);
            return _mapper.Map<PreSaleGroupDto[]>(preSaleGroups);
        }

        public async Task<PreSaleGroupDto> GetGroupAsync(CancellationToken cancellationToken, Guid id)
        {
            var preSaleGroup = await _context.PreSaleGroups
                .SingleOrDefaultAsync(psg => psg.Id == id, cancellationToken);

            if (preSaleGroup == null)
            {
                throw new NotFoundException();
            }

            return _mapper.Map<PreSaleGroupDto>(preSaleGroup);
        }

        public async Task<PreSaleGroupStatusDto[]> GetGroupStatusesAsync(CancellationToken cancellationToken)
        {
            var preSaleGroupStatuses = await _context.PreSaleGroupStatuses
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<PreSaleGroupStatusDto[]>(preSaleGroupStatuses);
        }

        public async Task<PreSaleGroup> CreateGroupAsync(CancellationToken cancellationToken, PreSaleGroupDto preSaleGroupDto)
        {
            _log.Info("Pre-sale group start Create");
            

            var preSaleGroup = _mapper.Map<PreSaleGroup>(preSaleGroupDto);

            preSaleGroup.CreatedDate = DateTime.UtcNow;
            preSaleGroup.ChangedDate = preSaleGroup.CreatedDate;

            await _context.PreSaleGroups.AddAsync(preSaleGroup, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            _log.Info($"Pre-sale group {preSaleGroup.Name} ({preSaleGroup.Id}) was created");

            return preSaleGroup;
        }

        public async Task EditGroupAsync(CancellationToken cancellationToken, Guid id, PreSaleGroupDto preSaleGroupDto)
        {
            _log.Info("Pre-sale group start Edit");

            if (id != preSaleGroupDto.Id)
            {
                throw new BadRequestException();
            }

            var preSaleGroup = _mapper.Map<PreSaleGroup>(preSaleGroupDto);
            
            preSaleGroup.ChangedDate = DateTime.UtcNow;

            _context.Entry(preSaleGroup).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync(cancellationToken);

                _log.Info($"Pre-sale group {preSaleGroup.Name} ({preSaleGroup.Id}) was edited");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GroupExistsAsync(cancellationToken, id))
                {
                    throw new NotFoundException();
                }

                throw;
            }
        }

        public async Task<PreSaleGroup> DeleteGroupAsync(CancellationToken cancellationToken, Guid id)
        {
            _log.Info("Pre-sale group start Delete");

            var preSaleGroup = await _context.PreSaleGroups
                .Include(psg => psg.Status)
                .Include(psg => psg.Department)
                .SingleOrDefaultAsync(psg => psg.Id == id, cancellationToken);

            if (preSaleGroup == null)
            {
                throw new NotFoundException();
            }

            _context.PreSaleGroups.Remove(preSaleGroup);
            await _context.SaveChangesAsync(cancellationToken);
            return preSaleGroup;
        }

        public async Task<bool> GroupExistsAsync(CancellationToken cancellationToken, Guid id) =>
            await _context.PreSaleGroups.AnyAsync(psg => psg.Id == id, cancellationToken);

        #endregion
    }
}