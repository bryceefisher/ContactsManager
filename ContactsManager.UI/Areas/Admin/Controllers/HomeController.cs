using ContactsManager.Core.Domain.Entities.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.UI.Areas.Admin.Controllers;



// specify the area name
[Area("Admin")]
// specify which roles can access this controller
[Authorize(Roles = "Admin")]
public class HomeController : Controller
{
    
    private readonly UserManager<ApplicationUser> _userManager;
    
    public HomeController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    
    //Can be applied to the controller or to the action
    //[Authorize(Roles = "Admin")]
    [Route("[Area]/[Controller]/[Action]")]
    // GET
    public IActionResult Index()
    {
        var users = _userManager.Users;
        return View(users);
    }
}