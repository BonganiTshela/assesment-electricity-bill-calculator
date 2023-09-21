using TarrifService.Models;

namespace TarrifService.Services.Util;

public class SinglePhaseBillCalculator
{
    private const double FreeEnergyConsumption = 100;
    private const double VAT = 15.0;

    /// <summary>
    /// Calculates the electricity bill based on a list of load profile readings.
    /// </summary>
    /// <param name="readings">The list of load profile readings.</param>
    /// <returns>A transaction object with the bill details.</returns>
    public static Transaction CalculateBill(List<LoadProfile> readings)
    {
        if (readings == null || readings.Count == 0)
            return new Transaction();

        readings = readings.OrderBy(r => r.ReadingTimeLocal).ToList();

        int? previousReading = null;
        int totalConsumption = 0;

        foreach (var reading in readings)
        {
            int currentReading = ToKWH(reading.ForwardActiveEnergyTypeReading);
            if (previousReading.HasValue)
            {
                var consumption = currentReading - previousReading.Value;
                totalConsumption += consumption;
            }
            previousReading = currentReading;
        }

        var amount = Math.Round(CalculateTotalCost(totalConsumption), 2);
        var amountVat = Math.Round(ApplyVAT(amount), 2);

        return new Transaction
        {
            EntityCode = "07604524236",
            Serial = "7604524236",
            CreatedDate = DateTime.Now,
            AmountExclVat = amount,
            AmountVat = amountVat,
            Consumption = totalConsumption
        };
    }

    /// <summary>
    /// Calculates the electricity bill using the initial and last reading from the list of load profile readings.
    /// </summary>
    /// <param name="readings">The list of load profile readings.</param>
    /// <returns>A transaction object with the bill details.</returns>
    public static Transaction CalculateBillSimple(List<LoadProfile> readings)
    {
        if (readings == null || readings.Count == 0)
            throw new ArgumentNullException("Load Profiles Not Provided!");

        readings = readings.OrderBy(r => r.ReadingTimeLocal).ToList();

        var initialReading = ToKWH(readings[0].ForwardActiveEnergyTypeReading);
        var lastReading = ToKWH(readings[readings.Count - 1].ForwardActiveEnergyTypeReading);

        var consumption = lastReading - initialReading;

        var amount = Math.Round(CalculateTotalCost(consumption), 2);
        var amountVat = Math.Round(ApplyVAT(amount), 2);

        return new Transaction
        {
            EntityCode = "07604524236",
            Serial = "7604524236",
            CreatedDate = DateTime.Now,
            AmountExclVat = amount,
            AmountVat = amountVat,
            Consumption = consumption
        };
    }

    private static int ToKWH(int value)
    {
        return value / 1000;
    }

    private static double ApplyVAT(double value)
    {
        var amount = value * (VAT / 100.0);
        return value + amount;
    }

    private static double CalculateTotalCost(double units)
    {
        var block1Rate = 2.4137;
        var block2Rate = 2.8247;
        var block3Rate = 3.0775;
        var block4Rate = 3.3179;

        var totalCost = 0.0;

        if (units <= 100)
        {
            totalCost = units * block1Rate;
        }
        else
        {
            totalCost = 100 * block1Rate;

            if (units <= 400)
            {
                totalCost += (units - 100) * block2Rate;
            }
            else
            {
                totalCost += 300 * block2Rate;

                if (units <= 650)
                {
                    totalCost += (units - 400) * block3Rate;
                }
                else
                {
                    totalCost += 250 * block3Rate;
                    totalCost += (units - 650) * block4Rate;
                }
            }
        }

        return totalCost;
    }
}