using CsvHelper;
using System.Globalization;
using TarrifService.Models;

namespace TarrifService.Services.Util;

public class CSVHelper
{
    /// <summary>
    /// No need for this to be generic on this app
    /// -- We are dealing with one object/type
    /// </summary>
    public static IEnumerable<T> ReadCSV<T>(Stream file)
    {
        var reader = new StreamReader(file);
        var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<LoadProfileMap>();

        var records = csv.GetRecords<T>();
        return records;
    }
}