using TarrifService.Models;

namespace TarrifService.Services.Util;

public class SinglePhaseBillCalculator
{
    private const double freeEnergyConsumption = 100;
    private const double VAT = 15.0;

    /// <summary>
    /// Calculates bill from readings then returns a transaction
    /// Notes: This calculation assumes a month period
    /// Do we also read the estimate readings?
    /// This method can be used if we need to evaulate each reading in the list (e.g Check the Reading type)
    /// Do we need to verify if the list is sorted by date/time?
    /// </summary>
    public static Transaction CalculateBill(List<LoadProfile> readings)
    {
        if (readings == null || readings.Count == 0)
            return new Transaction();

        readings = readings.OrderBy(r => r.ReadingTimeLocal).ToList();//This may or may not be required - based on requirements

        int? previousReading = null;
        int totalConsumption = 0;

        foreach (var reading in readings)
        {
            //if(reading.ForwardActiveEnergyType == "TookReading") -- I've assumed we also read the EstimatedReading?

            int currentReading = ToKWH(reading.ForwardActiveEnergyTypeReading);
            if (previousReading.HasValue)
            {
                var consumption = currentReading - previousReading.Value;
                totalConsumption += consumption;
            }
            previousReading = currentReading;
        }

        /*// INDIHENT: Subtract the first 100 kWh from the total energy consumption(need to be able yo identify indigent consumers)
        if (totalConsumption > freeEnergyConsumption)
        {
            totalConsumption -= freeEnergyConsumption;
        }*/

        var amount = Math.Round(CalculateTotalCost(totalConsumption), 2);
        var amountVat = Math.Round(ApplyVAT(amount), 2);

        return new Transaction
        {
            EntityCode = "07604524236"
            , Serial = "7604524236"
            , CreatedDate = DateTime.Now
            , AmountExclVat = amount
            , AmountVat = amountVat
            ,Consumption = totalConsumption
        };
    }

    /// <summary>
    /// This is a simpler verion of the calculation 
    /// uses the initial & the last reading
    /// Do we need to verify if the list is sorted by date/time?
    /// This method assumes all readings in the file are valid
    /// </summary>

    public static Transaction CalculateBillSimple(List<LoadProfile> readings)
    {
        if (readings == null || readings.Count == 0)
            throw new ArgumentNullException("Load Profiles Not Provided!");

        readings = readings.OrderBy(r => r.ReadingTimeLocal).ToList();//This may or may not be required - based on requirements

        var initalReading = ToKWH(readings[0].ForwardActiveEnergyTypeReading);
        var lastReading = ToKWH(readings[readings.Count - 1].ForwardActiveEnergyTypeReading);

        var consumption = lastReading - initalReading;
        /*// INDIHENT: Subtract the first 100 kWh from the total energy consumption(need to be able yo identify indigent consumers)
        if (consumption > freeEnergyConsumption)
        {
            totalConsumption -= freeEnergyConsumption;
        }*/

        var amount = Math.Round(CalculateTotalCost(consumption), 2);
        var amountVat = Math.Round(ApplyVAT(amount), 2);

        return new Transaction
        {
            EntityCode = "07604524236"
            , Serial = "7604524236"
            , CreatedDate = DateTime.Now
            , AmountExclVat = amount
            , AmountVat = amountVat
            ,Consumption = consumption
        };
    }

    public static int ToKWH(int value)
    {
        return value / 1000;
    }

    public static double ApplyVAT(double value)
    {
        var amount = value * (double)(VAT / 100.0);
        return value + amount;
    }

    public static double CalculateTotalCost(double units)
    {
        var block_1 = (241.37/100);
        var block_2 = (282.47/100);
        var block_3 = (307.75/100);
        var block_4 = (331.79/100);

        var totalCost = 0.0;

        if (units <= 100)
            totalCost = units * block_1; //Block 1 (units <= 100 kwh)
        else
        {
            totalCost = 100 * block_1;
            // Block 2 (101-400 kwh).
            if (units <= 400)
                totalCost += (units - 100) * block_2;
            else
            {
                totalCost += 300 * block_2; // (400-100) = 300 units in block 2.
                // Block 3 (401-650 kwh).
                if (units <= 650)
                    totalCost += (units - 400) * block_3;
                else
                {
                    totalCost += 250 * block_3; // (650-400) = 250 units in block 3.                   
                    totalCost += (units - 650) * block_4; // Block 4 (units > 650).
                }
            }
        }

        return totalCost;
    }
}
