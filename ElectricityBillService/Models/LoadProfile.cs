using CsvHelper.Configuration;

namespace TarrifService.Models;

public class LoadProfile
{

    public string? Id { get; set; }
    public string? Serial { get; set; }
    public DateTime ReadingTimeLocal { get; set; }
    public DateTime ReadingTimeUCT { get; set; }
    public string? ForwardActiveEnergyType { get; set; }
    public int ForwardActiveEnergyTypeReading { get; set; }

}

/// <summary>
/// Used to map CSV headers
/// </summary>
public sealed class LoadProfileMap : ClassMap<LoadProfile>
{
    public LoadProfileMap()
    {
        Map(m => m.Id).Ignore();
        Map(m => m.Serial).Name("Serial", "Serial"); ;
        Map(m => m.ReadingTimeLocal).Name("Time of Reading - Local", "ReadingTimeLocal");
        Map(m => m.ReadingTimeUCT).Name("Time of Reading - UTC", "ReadingTimeUCT");
        Map(m => m.ForwardActiveEnergyType).Name("forwardActiveEnergy Type", "ForwardActiveEnergyType");
        Map(m => m.ForwardActiveEnergyTypeReading).Name("forwardActiveEnergy Value", "ForwardActiveEnergyTypeReading");
    }
}
