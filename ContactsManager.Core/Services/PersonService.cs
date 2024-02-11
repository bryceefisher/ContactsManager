using System.Globalization;
using System.Reflection;
using System.Text;
using ContactsManager.Core.Domain.Entities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.Helpers;
using ContactsManager.Core.ServiceContracts;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;

namespace ContactsManager.Core.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository _personsRepository;
   

    //constructor
    public PersonService(IPersonRepository repo)
    {
        _personsRepository = repo;
        
    }


    /// <summary>
    /// Converts a Person object to personResponse object and adds county name based on country id.
    /// </summary>
    /// <param name="person"></param>
    /// <returns>Converted personResponse Object</returns>
    private PersonResponse ConvertPerson(Person person)
    {
        PersonResponse personResponse = person.ToPersonResponse();
        personResponse.Country = person.Country?.CountryName;
        return personResponse;
    }


    public async Task<PersonResponse> AddPerson(PersonAddRequest? request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        ValidationHelper.ModelValidation(request);

        Person person = request.ToPerson();

        await _personsRepository.AddPerson(person);
       

        return ConvertPerson(person);
    }


    public async Task<List<PersonResponse>> GetAllPeople(string userId)
    {
        return (await _personsRepository.GetAllPersons(userId)).Select(p => p.ToPersonResponse()).ToList();

       
    }


    public async Task<PersonResponse?> GetPersonByPersonId(int? personId, string userId)
    {
        if (personId is null)
        {
            return null;
        }

        return (await _personsRepository.GetPersonById(personId.Value, userId))?.ToPersonResponse() ?? null;

        
       
    }

    public async Task<List<PersonResponse>> GetFilteredPersons(string searchBy, string? searchString, string userId)
    {
        PropertyInfo? property = null;

        List<PropertyInfo> propertyInfo = typeof(PersonResponse).GetProperties().ToList();

        foreach (PropertyInfo prop in propertyInfo)
        {
            if (String.Equals(prop.ToString()?.Substring(prop.ToString().IndexOf(" ", StringComparison.Ordinal) + 1),
                    searchBy, StringComparison.OrdinalIgnoreCase))
            {
                property = prop;
                break;
            }
        }

        if (string.IsNullOrEmpty(searchString))
        {
            return (await _personsRepository.GetAllPersons(userId)).Select(p => p.ToPersonResponse()).ToList();
        }

        List<PersonResponse> convertedPeople = (await _personsRepository.GetAllPersons(userId)).Select(p => p.ToPersonResponse()).ToList();

        return convertedPeople.Where(p =>
                property?.GetValue(p)?.ToString()?.Contains(searchString, StringComparison.OrdinalIgnoreCase) ==
                true)
            .ToList();
    }

    public List<PersonResponse> GetSortedPeople(List<PersonResponse> allPeople, string sortBy, SortOrderEnum sortOrder)
    {
        if (String.IsNullOrEmpty(sortBy))
            return allPeople;

        List<PersonResponse> sortedPeople = sortOrder == SortOrderEnum.ASC
            ? sortedPeople = allPeople.OrderBy(p => p.GetType().GetProperty(sortBy)?.GetValue(p)).ToList()
            : sortedPeople = allPeople.OrderByDescending(p => p.GetType().GetProperty(sortBy)?.GetValue(p)).ToList();


        return sortedPeople;
    }

    public async Task<PersonResponse> UpdatePerson(PersonUpdateRequest? personUpdateRequest, string userId)
    {
        if (personUpdateRequest is null)
        {
            throw new ArgumentNullException(nameof(Person));
        }

        ValidationHelper.ModelValidation(personUpdateRequest);

        Person personToUpdate = (await _personsRepository.GetPersonById(personUpdateRequest.PersonId, userId))  ??
                                throw new ArgumentException(
                                    $"Given PersonId does not exist {nameof(personUpdateRequest.PersonId)}");

        personToUpdate.PersonId = personUpdateRequest.PersonId;
        personToUpdate.CountryId = personUpdateRequest.CountryId;
        personToUpdate.Address = personUpdateRequest.Address;
        personToUpdate.Email = personUpdateRequest.Email;
        personToUpdate.PersonName = personUpdateRequest.PersonName;

        
        return (await _personsRepository.UpdatePerson(personToUpdate, userId)).ToPersonResponse();
    }

    public async Task<bool> DeletePerson(int? personId, string userId)
    {
        if (personId is null)
        {
            return false;
        }

        Person? personToDelete = await _personsRepository.GetPersonById(personId.Value, userId);

        if (personToDelete is null)
        {
            return false;
        }

        await _personsRepository.DeletePerson(personId.Value, userId);
        
        return true;
    }


    public async Task<MemoryStream> GetPeopleCSV(string userId)
    {
        MemoryStream memoryStream = new MemoryStream();
        StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
        // CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        // {
        //     HasHeaderRecord = true,
        //     Delimiter = ",",
        //     IgnoreBlankLines = true,
        //     TrimOptions = TrimOptions.Trim,
        // };
        //Would need to add config instead of culture info below
        CsvWriter csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        // Write the header row with property names
        // csvWriter.WriteField(nameof(PersonResponse.PersonId));
        // csvWriter.WriteField(nameof(PersonResponse.PersonName));
        // csvWriter.WriteField(nameof(PersonResponse.Email));
        // csvWriter.WriteField(nameof(PersonResponse.DateOfBirth));
        // csvWriter.WriteField(nameof(PersonResponse.Gender));
        // csvWriter.WriteField(nameof(PersonResponse.Country));
        // csvWriter.WriteField(nameof(PersonResponse.Address));
        // csvWriter.WriteField(nameof(PersonResponse.ReceiveNewsLetters));
        csvWriter.WriteHeader<PersonResponse>();
        await csvWriter.NextRecordAsync();

        List<PersonResponse> people =
            (await _personsRepository.GetAllPersons(userId)).Select(p => p.ToPersonResponse()).ToList();

        foreach (var person in people)
        {
            //can write individual fields here
            csvWriter.WriteRecord(person);
            await csvWriter.NextRecordAsync();
        }

        await streamWriter.FlushAsync();
        memoryStream.Position = 0;

        return memoryStream;
    }

    public async Task<MemoryStream> GetPeopleExcel(string userId)
    {
        MemoryStream memoryStream = new();
        using (ExcelPackage excelPackage = new(memoryStream))
        {
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("PeopleSheet");
            worksheet.Cells["A1"].Value = "PersonId";
            worksheet.Cells["B1"].Value = "PersonName";
            worksheet.Cells["C1"].Value = "Email";
            worksheet.Cells["D1"].Value = "Phone";
            worksheet.Cells["E1"].Value = "DateOfBirth";
            worksheet.Cells["F1"].Value = "Country";
            worksheet.Cells["G1"].Value = "Address";
          

            int row = 2;

            List<PersonResponse> people =
                (await _personsRepository.GetAllPersons(userId)).Select(p => p.ToPersonResponse()).ToList();

            foreach (var person in people)
            {
                worksheet.Cells[row, 1].Value = person.PersonId;
                worksheet.Cells[row, 2].Value = person.PersonName;
                worksheet.Cells[row, 3].Value = person.Email;
                worksheet.Cells[row, 4].Value = person.Phone;
                worksheet.Cells[row, 5].Value = person.DateOfBirth;
                worksheet.Cells[row, 6].Value = person.Country;
                worksheet.Cells[row, 7].Value = person.Address;
                row++;
            }
            
            worksheet.Cells[$"A1:H{row}"].AutoFitColumns();
            
            await excelPackage.SaveAsync();
        }
        
        memoryStream.Position = 0;
        return memoryStream;
    }
    
}