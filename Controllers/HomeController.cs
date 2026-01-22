using Microsoft.AspNetCore.Mvc;
using CRM.Web.Services;
using System.Threading.Tasks;

namespace CRM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly DatabaseService _dbService;

        public HomeController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["UsersCount"] = await _dbService.GetUsersCountAsync();
            ViewData["ClientsCount"] = await _dbService.GetClientsCountAsync();
            ViewData["DealsCount"] = await _dbService.GetDealsCountAsync();
            ViewData["TotalAmount"] = await _dbService.GetTotalDealsAmountAsync();
            return View();
        }
    }
}