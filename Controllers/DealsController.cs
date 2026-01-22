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
        [ValidateAntiForgeryToken] // Этот атрибут должен быть
        public async Task<IActionResult> Edit(long id, Deal deal)
        {
            Console.WriteLine($"=== ОБНОВЛЕНИЕ СДЕЛКИ {id} ===");
            Console.WriteLine($"DealName: {deal.DealName}");
            Console.WriteLine($"ClientId: {deal.ClientId}");
            Console.WriteLine($"Amount: {deal.Amount}");
            Console.WriteLine($"Stage: {deal.Stage}");
            Console.WriteLine($"ResponsibleUserId: {deal.ResponsibleUserId}");
            Console.WriteLine($"DeadlineDate: {deal.DeadlineDate}");

            if (id != deal.Id)
            {
                Console.WriteLine("ОШИБКА: ID не совпадают");
                return NotFound();
            }

            // Проверяем ModelState
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ОШИБКИ ВАЛИДАЦИИ:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($" - {error.ErrorMessage}");
                }

                await PopulateViewBags();
                return View(deal);
            }

            try
            {
                var result = await _dbService.UpdateDealAsync(deal);

                if (result.Success)
                {
                    Console.WriteLine("✅ Сделка успешно обновлена");
                    return RedirectToAction("Index");
                }
                else
                {
                    Console.WriteLine($"❌ Ошибка обновления: {result.ErrorMessage}");
                    ModelState.AddModelError("", result.ErrorMessage);
                    await PopulateViewBags();
                    return View(deal);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Исключение: {ex.Message}");
                ModelState.AddModelError("", $"Ошибка при обновлении: {ex.Message}");
                await PopulateViewBags();
                return View(deal);
            }
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

            ViewBag.ClientsList = new SelectList(clientsList, "Id", "CompanyName");
            ViewBag.UsersList = new SelectList(usersList, "Id", "LastName");

            ViewBag.Clients = clientsList;
            ViewBag.Users = usersList;
        }
    }
}