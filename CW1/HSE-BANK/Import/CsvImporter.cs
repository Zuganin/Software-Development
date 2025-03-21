using System.Globalization;
using CsvHelper;
using HSE_BANK.Domain_Models;

namespace HSE_BANK.Import;

public class CsvImporter<T> : DataImporter<T>
{
    public override List<T> ParseData(string fileContent)
    {
        using (var reader = new StringReader(fileContent))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = csv.GetRecords<T>().ToList();
            return records;
        }
    }
}