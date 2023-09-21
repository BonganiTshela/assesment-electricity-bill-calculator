using Microsoft.AspNetCore.Mvc;
using TarrifService.Models;
using TarrifService.Services.Util;

namespace TarrifService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ElectricityBillController : ControllerBase
{

    [HttpPost("upload-load-profiles-csv")]
    public ActionResult<Transaction> CalculateBill([FromForm] IFormFileCollection file)
    {
        var loadProfiles = CSVHelper.ReadCSV<LoadProfile>(file[0].OpenReadStream()).ToList();
        var transaction = SinglePhaseBillCalculator.CalculateBill(loadProfiles);

        return Ok(transaction);
    }
}
