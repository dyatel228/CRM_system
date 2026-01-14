using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CRM.Web.Models;
using CRM.Web.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Web.Controllers
{
    public class DealsController : Controller
    {
        private readonly DatabaseService _dbService;

        public DealsController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        // GET: /Deals
        public async Task<IActionResult> Index()
        {
            var deals = await _dbService.GetAllDealsAsync();
            return View(deals);
        }

        // GET: /Deals/Create
        public async Task<IActionResult> Create()
        {
            await PopulateViewBags();
            return View();
        }

        // POST: /Deals/Create
        [HttpPost]
        public async Task<IActionResult> Create(Deal deal)
        {
            if (ModelState.IsValid)
            {
                await _dbService.AddDealAsync(deal);
                return RedirectToAction("Index");
            }

            await PopulateViewBags();
            return View(deal);
        }

        // GET: /Deals/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var deal = await _dbService.GetDealByIdAsync(id);
            if (deal == null)
            {
                return NotFound();
            }

            await PopulateViewBags();
            return View(deal);
        }

        // POST: /Deals/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(long id, Deal deal)
        {
            if (id != deal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _dbService.UpdateDealAsync(deal);
                return RedirectToAction("Index");
            }

            await PopulateViewBags();
            return View(deal);
        }

        // GET: /Deals/Delete/5
        public async Task<IActionResult> Delete(long id)
        {
            var deal = await _dbService.GetDealByIdAsync(id);
            if (deal == null)
            {
                return NotFound();
            }
            return View(deal);
        }

        // POST: /Deals/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            await _dbService.DeleteDealAsync(id);
            return RedirectToAction("Index");
        }

        private async Task PopulateViewBags()
        {
            // Получаем списки
            var clientsList = await _dbService.GetClientsForDropdownAsync();
            var usersList = await _dbService.GetUsersForDropdownAsync();

            // Создаем SelectList для ViewBag
            ViewBag.ClientsList = new SelectList(clientsList, "Id", "CompanyName");
            ViewBag.UsersList = new SelectList(usersList, "Id", "LastName");

            // Также сохраняем оригинальные списки для использования в цикле
            ViewBag.Clients = clientsList;
            ViewBag.Users = usersList;
        }
    }
}