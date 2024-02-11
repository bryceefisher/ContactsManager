using ContactsManager.Core.Domain.Entities.IdentityEntities;
using ContactsManager.Core.DTO;
using ContactsManager.Core.Enums;
using ContactsManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

namespace ContactsManager.UI.Controllers;

[Route("persons/[action]")]
public class PersonController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPersonService _personService;
    private readonly ICountriesService _countriesService;


    public PersonController(IPersonService personService, ICountriesService countriesService, UserManager<ApplicationUser> userManager)
    {
        _personService = personService;
        _countriesService = countriesService;
        _userManager = userManager;
        
    }

    // GET
    [HttpGet]
    [Route("/")]
    public async Task<IActionResult> Index(string? searchBy, string? searchString, string sortBy = "PersonName",
        SortOrderEnum sortOrder = SortOrderEnum.ASC)
    {
        
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        

        //Populate search field
        ViewBag.SearchFields = new Dictionary<string, string>()
        {
            { nameof(PersonResponse.PersonName), "Person Name" },
            { nameof(PersonResponse.Email), "Email" },
            { nameof(PersonResponse.DateOfBirth), "Date of Birth" },
            {nameof(PersonResponse.Phone), "Phone"},
            { nameof(PersonResponse.Gender), "Gender" },
            { nameof(PersonResponse.Country), "Country" },
            { nameof(PersonResponse.Address), "Address" },
        };
        //Filter list of people, if search string null return entire list
        
        List<PersonResponse> people = await _personService.GetFilteredPersons(searchBy, searchString, userId);

        //sort list people, if sort by null sort by person name ASC
        people = _personService.GetSortedPeople(people, sortBy, sortOrder);

        ViewBag.CurrentSearchBy = searchBy;
        ViewBag.CurrentSearchString = searchString;

        ViewBag.CurrentSortBy = string.IsNullOrEmpty(sortBy) ? "PersonName" : sortBy;
        ViewBag.CurrentSortOrder = sortOrder;

        return View(people);
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
            { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        return View();
    }


    [HttpPost]
    public async Task<IActionResult> Create(PersonAddRequest personRequest)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
        personRequest.UserId = userId;
        
        if (!ModelState.IsValid)
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();

            ViewBag.Countries = countries.Select(temp => new SelectListItem()
                { Text = temp.CountryName, Value = temp.CountryID.ToString() });

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

            return View();
        }

        PersonAddRequest person = new PersonAddRequest()
        {
            PersonName = personRequest.PersonName,
            Email = personRequest.Email,
            Phone = personRequest.Phone,
            DateOfBirth = personRequest.DateOfBirth,
            CountryId = personRequest.CountryId,
            Address = personRequest.Address,
            UserId = personRequest.UserId
        };

        await _personService.AddPerson(person);


        return RedirectToAction("Index", "Person");
    }

    [HttpGet]
    [Route("{personId}")]
    public async Task<IActionResult> Edit(int personId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
        PersonResponse personResponse = await _personService.GetPersonByPersonId(personId, userId);
        PersonUpdateRequest personToUpdate = personResponse.ToPersonUpdateRequest();

        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
            { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

        return View(personToUpdate);
    }

    [HttpPost]
    [Route("{personId}")]
    public async Task<IActionResult> Edit(PersonUpdateRequest person)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
        person.UserId = userId;
        
        if (ModelState.IsValid)
        {
            await _personService.UpdatePerson(person, userId);

            return RedirectToAction("Index", "Person");
        }
        
        List<CountryResponse> countries = await _countriesService.GetAllCountries();

        ViewBag.Countries = countries.Select(temp => new SelectListItem()
            { Text = temp.CountryName, Value = temp.CountryID.ToString() });

        return View();
    }

    [HttpGet]
    [Route("{personId}")]
    public async Task<IActionResult> Delete(int personId)
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
        PersonResponse? personToDelete = await _personService.GetPersonByPersonId(personId, userId);
        if (ModelState.IsValid && personToDelete is not null)
        {
            return View(personToDelete);
        }

        return RedirectToAction("Index", "Person");
    }

    [HttpPost]
    [Route("{personId}")]
    public async Task<IActionResult> Delete(PersonResponse? person)
    {
        
    var currentUser = await _userManager.GetUserAsync(User);
    if (currentUser is null)
    {
        return RedirectToAction("Login", "Account");
    }
    var userId = currentUser?.Id;
    
    PersonResponse? personToDelete = await _personService.GetPersonByPersonId(person.PersonId, userId);
    
        if (personToDelete is not null)
        {
            bool isDeleted = await _personService.DeletePerson(personToDelete.PersonId, userId);

            if (isDeleted)
            {
                return RedirectToAction("Index", "Person");
            }

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return View(person);
        }


        return View(person);
    }

    
    public async Task<IActionResult> PeoplePDF()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
       List<PersonResponse> people =  await _personService.GetAllPeople(userId);

       return new ViewAsPdf("PeoplePDF", people, ViewData)
       {
           PageMargins = new Margins() {Top = 20, Bottom = 20, Left = 20, Right = 20},
           PageOrientation = Orientation.Landscape,
       };
    }

    public async Task<IActionResult> PeopleCSV()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
       MemoryStream memoryStream =  await _personService.GetPeopleCSV(userId);
       
       return File(memoryStream, "application/octet-stream", "People.csv");
    }
    
    public async Task<IActionResult> PeopleExcel()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser is null)
        {
            return RedirectToAction("Login", "Account");
        }
        var userId = currentUser?.Id;
        
        MemoryStream memoryStream =  await _personService.GetPeopleExcel(userId);

        return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "People.xlsx");

    }
    
}