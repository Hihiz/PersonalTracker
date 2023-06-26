using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonalTracker.Data;
using PersonalTracker.Models;
using System.Globalization;

namespace PersonalTracker.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationContext _db;

        public DashboardController(ApplicationContext db) => _db = db;

        public async Task<ActionResult> Index()
        {
            DateTime startDate = DateTime.Today.AddDays(-6);
            DateTime endDate = DateTime.Today;

            List<Transaction> selectedTransactions = await _db.Transactions
                .Include(t => t.Category)
                .Where(t => t.Date >= startDate && t.Date <= endDate)
                .ToListAsync();

            // общий доход
            int totalIncome = selectedTransactions
                .Where(i => i.Category.Type == "Доход")
                .Sum(i => i.Amount);
            ViewBag.TotalIncome = totalIncome.ToString("c0");

            // общий расход
            int totalExpense = selectedTransactions
              .Where(i => i.Category.Type == "Расход")
              .Sum(i => i.Amount);
            ViewBag.TotalExpense = totalExpense.ToString("c0");

            //баланс
            int balance = totalIncome - totalExpense;
            CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture("en-US");
            cultureInfo.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = balance.ToString("c0");

            ViewBag.DoughnutChartData = selectedTransactions
                  .Where(i => i.Category.Type == "Расход")
                  .GroupBy(j => j.Category.Id)
                  .Select(k => new
                  {
                      categoryTitleWithIcon = k.First().Category.Icon + " " + k.First().Category.Title,
                      amount = k.Sum(j => j.Amount),
                      formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                  })
                  .OrderByDescending(l => l.amount)
                  .ToList();

            List<SplineChartData> incomeSummary = selectedTransactions
                .Where(i => i.Category.Type == "Доход")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Income = k.Sum(l => l.Amount)
                }).ToList();

            List<SplineChartData> expenseSummary = selectedTransactions
                .Where(i => i.Category.Type == "Расход")
                .GroupBy(j => j.Date)
                .Select(k => new SplineChartData()
                {
                    Day = k.First().Date.ToString("dd-MMM"),
                    Expense = k.Sum(l => l.Amount)
                }).ToList();

            string[] last7Days = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i).ToString("dd-MMM"))
                .ToArray();

            ViewBag.SplineChartData = from day in last7Days
                                      join income in incomeSummary on day equals income.Day into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in expenseSummary on day equals expense.Day into expenseJoined
                                      from expense in expenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.Income,
                                          expense = expense == null ? 0 : expense.Expense,
                                      };

            ViewBag.RecentTransactions = await _db.Transactions
               .Include(i => i.Category)
               .OrderByDescending(j => j.Date)
               .Take(5)
               .ToListAsync();

            return View();
        }

        public class SplineChartData
        {
            public string Day;
            public int Income;
            public int Expense;
        }
    }
}