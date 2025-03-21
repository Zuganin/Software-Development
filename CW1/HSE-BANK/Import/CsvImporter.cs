using System.Globalization;
using CsvHelper;

namespace HSE_BANK.Import;

public class CsvImporter<T> : DataImporter<T>
{
    public override List<T> ParseData(string fileContent)
    {
        using (var reader = new StringReader(fileContent))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {

            return new List<T>(csv.GetRecords<T>());
        }
    }
}