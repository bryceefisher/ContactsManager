using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace ContactsManager.Core.Services;

public class CountriesService : ICountriesService
{
    //private list of countries
    private readonly ICountriesRepository _countriesRepository;

    public CountriesService(ICountriesRepository repo)
    {
        _countriesRepository = repo;
    }

    public async Task<CountryResponse> AddCountry(CountryAddRequest? request)
    {
        //validation country not null
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        //convert object from CountryAddRequest to Country type
        Country country = request.ToCountry();

        if (country.CountryName == null)
        {
            throw new ArgumentNullException(nameof(country.CountryName));
        }

        if (await _countriesRepository.GetCountryByName(request.CountryName) != null)
        {
            throw new ArgumentException("Country name already exists");
        }


        //generate new CountryId


        await _countriesRepository.AddCountry(country);
        
        return country.ToCountryResponse();
    }


    public async Task<List<CountryResponse>> GetAllCountries()
    {
        return (await _countriesRepository.GetAllCountries()).Select(country => country.ToCountryResponse()).ToList();
    }


    public async Task<CountryResponse>? GetCountryByCountryId(int? countryId)
    {
        if (countryId == null)
        {
            return null;
        }

        Country? countryToReturn = await _countriesRepository.GetCountryById(countryId.Value);

        if (countryToReturn == null)
            return null;

        return countryToReturn.ToCountryResponse();
    }

    public async Task<int> CountryUploadFromFile(IFormFile file)
    {
        MemoryStream ms = new();
        await file.CopyToAsync(ms);
        int countriesInserted = 0;
        using (ExcelPackage excelPackage = new ExcelPackage(ms))
        {
            ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets["Countries"];

            int rowCount = workSheet.Dimension.Rows;

            for (int i = 2; i <= rowCount; i++)
            {
                string? cellValue = Convert.ToString(workSheet.Cells[i, 1].Value);

                if (!string.IsNullOrEmpty(cellValue))
                {
                    string? countryName = cellValue;

                    if (await _countriesRepository.GetCountryByName(countryName) is null)
                    {
                        Country country = new()
                        {
                            CountryName = countryName
                        };
                        await _countriesRepository.AddCountry(country);
                        
                        countriesInserted++;
                    }
                }
            }
        }

        return countriesInserted;
    }
}