namespace TarrifService.Models;

public class Transaction
{

    public string? Id { get; set; }
    public string? Serial { get; set; }
    public string? EntityCode { get; set; }
    public DateTime? CreatedDate { get; set; }
    public double AmountExclVat { get; set; }
    public double AmountVat { get; set; }
    public double Consumption { get; set; }
    public double BasicCharge { get; set; }
}


