using Microsoft.AspNetCore.Mvc;
using CRM.Web.Models;
using CRM.Web.Services;
using System.Threading.Tasks;

namespace CRM.Web.Controllers
{
    public class ClientsController : Controller
    {
        private readonly DatabaseService _dbService;

        public ClientsController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // GET: /Clients
        public async Task<IActionResult> Index()
        {
            var clients = await _dbService.GetAllClientsAsync();
            return View(clients);
        }

        // GET: /Clients/Create
        public async Task<IActionResult> Create()
        {
            await PopulateUsers();
            return View();
        }

        // POST: /Clients/Create
        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                await _dbService.AddClientAsync(client);
                return RedirectToAction("Index");
            }

            await PopulateUsers();
            return View(client);
        }

        // GET: /Clients/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var client = await _dbService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            await PopulateUsers();
            return View(client);
        }

        // POST: /Clients/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(long id, Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dbService.UpdateClientAsync(client);
                return RedirectToAction("Index");
            }

            await PopulateUsers();
            return View(client);
        }

        // GET: /Clients/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var client = await _dbService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: /Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _dbService.DeleteClientAsync(id);
            return RedirectToAction("Index");
        }

        private async Task PopulateUsers()
        {
            var users = await _dbService.GetUsersForDropdownAsync();
            ViewBag.Users = users;
        }
    }
}