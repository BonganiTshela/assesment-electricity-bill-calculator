using TarrifService.Services.Util;
using TarrifService.Models;

namespace ElectricityBillService.Tests;

[TestFixture]
public class SinglePhaseBillCalculatorTests
{
    [Test]
    public void CalculateBill_Block_1()
    {
        var readings = new List<LoadProfile>
        {
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 0 },
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 100000 }
        };

        var transaction = SinglePhaseBillCalculator.CalculateBill(readings);

        Assert.IsNotNull(transaction);
        Assert.That(transaction.Consumption, Is.EqualTo(100)); 
        Assert.That(transaction.AmountExclVat, Is.EqualTo(241.37)); 
    }

    [Test]
    public void CalculateBill_Block_2()
    {
        var readings = new List<LoadProfile>
        {
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 100000 },
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 450000 }
        };

        var transaction = SinglePhaseBillCalculator.CalculateBillSimple(readings);

        Assert.IsNotNull(transaction);
        Assert.That(transaction.Consumption, Is.EqualTo(350));
        Assert.That(transaction.AmountExclVat, Is.EqualTo(947.54));
    }

    [Test]
    public void CalculateBill_Block_3()
    {
        var readings = new List<LoadProfile>
        {
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 0 },
            new LoadProfile { ReadingTimeLocal = DateTime.Now.AddHours(6), ForwardActiveEnergyTypeReading = 450000 },
        };

        var transaction = SinglePhaseBillCalculator.CalculateBill(readings);

        Assert.IsNotNull(transaction);
        Assert.That(transaction.Consumption, Is.EqualTo(450));
        Assert.That(transaction.AmountExclVat, Is.EqualTo(1242.66));
    }

    [Test]
    public void CalculateBill_Block_4()
    {
        var readings = new List<LoadProfile>
        {
            new LoadProfile { ReadingTimeLocal = DateTime.Now, ForwardActiveEnergyTypeReading = 0 },
            new LoadProfile { ReadingTimeLocal = DateTime.Now.AddHours(1), ForwardActiveEnergyTypeReading = 1000000 }
        };

        var transaction = SinglePhaseBillCalculator.CalculateBill(readings);

        Assert.IsNotNull(transaction);
        Assert.That(transaction.Consumption, Is.EqualTo(1000));
        Assert.That(transaction.AmountExclVat, Is.EqualTo(3019.42));
    }

    [Test]
    public void CalculateBill_Block_EmptyLoapdProfile()
    {
        var readings = new List<LoadProfile>();
        var transaction = SinglePhaseBillCalculator.CalculateBill(readings);

        Assert.IsNotNull(transaction);
        Assert.That(transaction.Consumption, Is.EqualTo(0));
        Assert.That(transaction.AmountExclVat, Is.EqualTo(0));
    }
}
