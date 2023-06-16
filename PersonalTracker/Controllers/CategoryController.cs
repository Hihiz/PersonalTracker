using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTracker.Data;
using PersonalTracker.Models;

namespace PersonalTracker.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationContext _db;

        public CategoryController(ApplicationContext db) => _db = db;

        public async Task<IActionResult> Index()
        {
            return _db.Categories != null ?
                 View(await _db.Categories.ToListAsync()) :
                 Problem("Categories is null");
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _db.Categories.FirstOrDefaultAsync(t => t.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        public IActionResult AddEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                return View(_db.Categories.Find(id));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit([Bind("Id,Title,Icon,Type")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    _db.Add(category);
                }
                else
                {
                    _db.Update(category);
                }

                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Category category = await _db.Categories.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Categories == null)
            {
                return Problem("Categories is null");
            }

            Category category = await _db.Categories.FindAsync(id);

            if (category != null)
            {
                _db.Categories.Remove(category);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}