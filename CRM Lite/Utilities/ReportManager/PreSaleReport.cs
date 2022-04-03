using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CRM_Lite.Data;
using CRM_Lite.Data.Models.PreSale;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Drawing.Chart;
using OfficeOpenXml.Style;
using Vostok.Logging.Abstractions;

namespace CRM_Lite.Utilities.ReportManager
{
    public class PreSaleReport : IPreSaleReport
    {
        private readonly ILog _log;
        private readonly ApplicationContext _applicationContext;
        
        private const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public PreSaleReport(ILog log, ApplicationContext applicationContext)
        {
            _log = log;
            _applicationContext = applicationContext;
        }

        public async Task<FileStreamResult> CreatePreSaleReportAsync(
            CancellationToken cancellationToken,
            PreSaleGroup preSaleGroup)
        {
            var preSales = await _applicationContext.PreSales
                .Where(ps => ps.GroupId == preSaleGroup.Id)
                .ToArrayAsync(cancellationToken);

            var preSaleResults = await _applicationContext.PreSaleResults.ToArrayAsync(cancellationToken);
            var preSaleStatuses = await _applicationContext.PreSaleStatuses.ToArrayAsync(cancellationToken);

            var regionInWork = new HashSet<Guid?>();
            var nowTime = DateTime.Now;

            var preSaleStatusStats = preSaleStatuses
                .ToDictionary(pss => pss.Name, pss => preSales.Count(ps => ps.StatusId == pss.Id));

            var preSaleResultStats = preSaleResults
                .ToDictionary(psr => psr.Name, psr => preSales.Count(ps => ps.ResultId == psr.Id));

            foreach (var ps in preSales)
                regionInWork.Add(ps.RegionId);

            var stream = new MemoryStream();

            using var package = new ExcelPackage(stream);
            var workSheet = package.Workbook.Worksheets.Add("Лист 1");

            workSheet.Cells["A1"].Value = preSaleGroup.Name;
            workSheet.Cells["C2"].Value = "Статус на";
            workSheet.Cells["D2"].Value = $"{nowTime.Day}.{nowTime.Month}.{nowTime.Year}";
            workSheet.Cells["A3"].Value = "Регионов в работе";
            workSheet.Cells["B3"].Value = regionInWork.Count;
            workSheet.Cells["A4"].Value = "Всего отправлено предложений";
            workSheet.Cells["B4"].Value = preSales.Length;
            workSheet.Cells["A5"].Value = "Передано сейлу";
            workSheet.Cells["B5"].Value = preSaleStatusStats["Передано сейлу"];
            workSheet.Cells["A6"].Value = "В работе";
            workSheet.Cells["B6"].Value = preSaleStatusStats["В работе"];
            workSheet.Cells["A7"].Value = "Не интересно";
            workSheet.Cells["B7"].Value = preSaleStatusStats["Не интересно"];

            workSheet.Cells["A9"].Value = "Сейлы:";
            workSheet.Cells["A10"].Value = "В работе";
            workSheet.Cells["B10"].Value = preSaleResultStats["В работе"];
            workSheet.Cells["A11"].Value = "Рассмотрели, но отказали";
            workSheet.Cells["B11"].Value = preSaleResultStats["Рассмотрели, но отказали"];
            workSheet.Cells["A12"].Value = "Договорились на демонстрацию";
            workSheet.Cells["B12"].Value = preSaleResultStats["Договорились на демонстрацию"];
            workSheet.Cells["A13"].Value = "Успешно";
            workSheet.Cells["B13"].Value = preSaleResultStats["Успешно"];

            workSheet.Cells[1, 1, 13, 2].AutoFitColumns();

            workSheet.Column(2).Width = 10.33;
            workSheet.Column(4).Width = 10.33;

            workSheet.Cells["A1:C1"].Merge = true;
            workSheet.Cells["A2:B2"].Merge = true;

            workSheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells["D2"].Style.Numberformat.Format = "dd.mm.yyyy";
            workSheet.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            workSheet.Cells["A2:D13"].Style.Font.Size = 10.0f;
            workSheet.Cells["B3:B13"].Style.Font.Bold = true;
            workSheet.Cells["B3:B13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            workSheet.Cells["A9"].Style.Font.Italic = true;
            workSheet.Cells["A1"].Style.Font.Size = 12.0f;
            workSheet.Cells["A1"].Style.Font.Bold = true;

            var statusPieChart = workSheet.Drawings.AddChart("statusPieChart", eChartType.Pie) as ExcelPieChart;
            statusPieChart.Title.Text = "Pre-sale";
            statusPieChart.Series.Add(ExcelCellBase.GetAddress(5, 2, 7, 2), ExcelCellBase.GetAddress(5, 1, 7, 1));
            statusPieChart.Legend.Position = eLegendPosition.Bottom;
            statusPieChart.DataLabel.ShowPercent = true;
            statusPieChart.SetSize(400, 400);
            statusPieChart.SetPosition(14, 0, 0, 0);

            var resultPieChart = workSheet.Drawings.AddChart("resultPieChart", eChartType.Pie) as ExcelPieChart;
            resultPieChart.Title.Text = "Сейл";
            resultPieChart.Series.Add(ExcelCellBase.GetAddress(10, 2, 13, 2), ExcelCellBase.GetAddress(10, 1, 13, 1));
            resultPieChart.Legend.Position = eLegendPosition.Bottom;
            resultPieChart.DataLabel.ShowPercent = true;
            resultPieChart.SetSize(400, 400);
            resultPieChart.SetPosition(14, 0, 4, 0);

            var salesFunnel = workSheet.Drawings.AddChart("salesFunnel", eChartType.ColumnClustered);
            salesFunnel.Title.Text = preSaleGroup.Name;
            var values = workSheet.Cells["B4,B10:B13"];
            var xvalues = workSheet.Cells["A4,A10:A13"];
            salesFunnel.Series.Add(values, xvalues);
            salesFunnel.Legend.Remove();
            salesFunnel.SetSize(400, 400);
            salesFunnel.SetPosition(14, 0, 11, 0);

            package.Save();

            stream.Position = 0;
            var name = $"Отчет по Pre-sale-рассылкам на {nowTime.Day}.{nowTime.Month}.{nowTime.Year}.xlsx";

            return new FileStreamResult(stream, ContentType)
                {FileDownloadName = name};
        }
    }
}