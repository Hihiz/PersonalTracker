using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PersonalTracker.Models;
using PersonalTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace PersonalTracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationContext _db;

        public TransactionController(ApplicationContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            var applicationContext = _db.Transactions.Include(t => t.Category);

            return View(await applicationContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Transaction transaction = await _db.Transactions.Include(t => t.Category).FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        public IActionResult AddEdit(int id = 0)
        {
            //ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title");

            PopulateCategories();

            if (id == 0)
            {
                return View(new Transaction());
            }
            else
            {
                return View(_db.Transactions.Find(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit([Bind("Id,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                if (transaction.Id == 0)
                {
                    _db.Add(transaction);
                }
                else
                {
                    _db.Update(transaction);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            //ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title", transaction.CategoryId);
            PopulateCategories();

            return View(transaction);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmer(int id)
        {
            if (_db.Transactions == null)
            {
                return Problem("Transaction is null");
            }

            Transaction transaction = await _db.Transactions.FindAsync(id);

            if (transaction != null)
            {
                _db.Transactions.Remove(transaction);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        public void PopulateCategories()
        {
            var categoryList = _db.Categories.ToList();
            Category defaultCategory = new Category() { Id = 0, Title = "Выберите категорию" };
            categoryList.Insert(0, defaultCategory);

            ViewBag.Categories = categoryList;
        }
    }
}