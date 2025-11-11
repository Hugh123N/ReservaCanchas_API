using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using ClosedXML.Excel;

namespace Acopio.Liquidaciones.Common
{
    public class GenerarExcel
    {
        public byte[] generar(Dictionary<string, object> datos, string rutaPlantilla)
        {
            try
            {
                using (var workbook = new XLWorkbook(rutaPlantilla))
                using (var memoryStream = new MemoryStream())
                {
                    var worksheet = workbook.Worksheet(1);

                    foreach (var item in datos)
                    {
                        if (Regex.IsMatch(item.Key, @"^[A-Z]+[0-9]+$"))
                        {
                            worksheet.Cell(item.Key).Value = item.Value.ToString();
                        }
                    }

                    workbook.SaveAs(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al generar Excel: {ex.Message}");
                return null;
            }
        }
    }
}
