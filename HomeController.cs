using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollMvc.Data;
using PayrollMvc.Models;

namespace PayrollMvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        public HomeController(AppDbContext db) => _db = db;

        public async Task<IActionResult> Index()
        
        {
            var totalEmployees = await _db.Employees.CountAsync();
            var latestPeriod = await _db.PayrollPeriods
                .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month)
                .FirstOrDefaultAsync();

            decimal totalPayroll = 0;
            decimal avgNet = 0;
            List<DashboardRow> recent = new();

            if (latestPeriod != null)
            {
                var entries = await _db.PayrollEntries
                    .Include(e => e.Employee)
                    .Where(e => e.PayrollPeriodId == latestPeriod.Id)
                    .OrderBy(e => e.Employee!.FullName)
                    .ToListAsync();

                if (entries.Count > 0)
                {
                    totalPayroll = entries.Sum(e => e.NetPay);
                    avgNet = Math.Round(entries.Average(e => e.NetPay), 0);

                    recent = entries.Take(10).Select(e => new DashboardRow
                    {
                        FullName = e.Employee!.FullName,
                        Department = e.Employee!.Department,
                        Title = e.Employee!.Title,
                        BaseSalary = e.Employee!.BaseSalary,
                        Bonus = e.Bonus,
                        NetPay = e.NetPay
                    }).ToList();
                }
            }

            ViewBag.TotalEmployees = totalEmployees;
            ViewBag.TotalPayroll = totalPayroll;
            ViewBag.AvgNet = avgNet;
            ViewBag.LatestPeriod = latestPeriod;
            ViewBag.RecentEntries = recent;

            return View();
        }
    }
}
