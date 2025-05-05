using ClosedXML.Excel;
using ConcessionariaManager.Core.Models.Dashboard;
using ConcessionariaManager.Web.Core.Interfaces.Services;

namespace ConcessionariaManager.Web.ExternalServices
{
    public class ClosedXMLExcelExportService : IExcelExportService
    {
        public byte[] ExportarRelatorio(List<RelatorioVendaItemViewModel> relatorio)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Relatório de Vendas");

            worksheet.Cell(1, 1).Value = "Tipo Veículo";
            worksheet.Cell(1, 2).Value = "Fabricante";
            worksheet.Cell(1, 3).Value = "Concessionária";
            worksheet.Cell(1, 4).Value = "Quantidade Vendida";
            worksheet.Cell(1, 5).Value = "Total Vendido";

            for (int i = 0; i < relatorio.Count; i++)
            {
                var item = relatorio[i];
                worksheet.Cell(i + 2, 1).Value = item.TipoVeiculo;
                worksheet.Cell(i + 2, 2).Value = item.Fabricante;
                worksheet.Cell(i + 2, 3).Value = item.Concessionaria;
                worksheet.Cell(i + 2, 4).Value = item.QuantidadeVendida;
                worksheet.Cell(i + 2, 5).Value = item.TotalVendido;
                worksheet.Cell(i + 2, 5).Style.NumberFormat.Format = "R$ #,##0.00";
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
