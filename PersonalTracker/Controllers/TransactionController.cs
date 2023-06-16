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

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _db.Add(transaction);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title", transaction.CategoryId);

            return View(transaction);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.Transactions == null)
            {
                return NotFound();
            }

            Transaction transaction = await _db.Transactions.FindAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title", transaction.CategoryId);

            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Amount,Note,Date")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(transaction);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Title", transaction.CategoryId);

            return View(transaction);
        }

        public async Task<IActionResult> Delete(int id)
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
    }
}