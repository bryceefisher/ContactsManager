using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts;

namespace CRUDExample.Controllers;

[Route("[Controller]/[action]")]
public class CountryController : Controller
{
    private readonly ICountriesService _countriesService;


    public CountryController(ICountriesService countriesService)
    {
        _countriesService = countriesService;
    }

    // GET
    public IActionResult UploadFromExcel()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> UploadFromExcel(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            @ViewBag.ErrorMessage = "File is empty";
            return View();
        }

        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            @ViewBag.ErrorMessage = "File is not an Excel file";
        }
        
        

        int numRowsAffected = await _countriesService.CountryUploadFromFile(file);

        ViewBag.SuccessMessage = $"{numRowsAffected} rows affected";
        return View();
    }
}