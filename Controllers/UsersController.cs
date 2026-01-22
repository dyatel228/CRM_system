using Microsoft.AspNetCore.Mvc;
using CRM.Web.Models;
using CRM.Web.Services;
using System.Threading.Tasks;

namespace CRM.Web.Controllers
{
    public class UsersController : Controller
    {
        private readonly DatabaseService _dbService;

        public UsersController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // GET: /Users
        public async Task<IActionResult> Index()
        {
            var users = await _dbService.GetAllUsersAsync();
            return View(users);
        }

        // GET: /Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Users/Create
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                var result = await _dbService.AddUserAsync(user);

                if (result.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Email", result.ErrorMessage);
                }
            }

            return View(user);
        }

        // GET: /Users/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var user = await _dbService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /Users/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(long id, User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _dbService.UpdateUserAsync(user);

                if (result.Success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Email", result.ErrorMessage);
                }
            }

            return View(user);
        }

        // GET: /Users/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _dbService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _dbService.DeleteUserAsync(id);
            return RedirectToAction("Index");
        }
    }
}