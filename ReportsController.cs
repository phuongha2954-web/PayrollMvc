using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollMvc.Data;
using PayrollMvc.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollMvc.Controllers;

public class ReportsController : Controller
{
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) => _db = db;

    // Danh sách kỳ lương để chọn xem/ in
    public async Task<IActionResult> Index()
    {
        var periods = await _db.PayrollPeriods
            .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month)
            .Select(p => new
            {
                p.Id,
                p.Month,
                p.Year,
                Entries = _db.PayrollEntries.Count(e => e.PayrollPeriodId == p.Id)
            })
            .ToListAsync();

        return View(periods); // View dynamic/anonymous OK trong bảng đơn giản
    }

    // Báo cáo chi tiết 1 kỳ lương (id=0 -> lấy kỳ mới nhất)
    [HttpGet("/Reports/PayrollSummary/{id?}")]
    public async Task<IActionResult> PayrollSummary(int? id)
    {
        var period = id.HasValue && id.Value > 0
            ? await _db.PayrollPeriods.FindAsync(id.Value)
            : await _db.PayrollPeriods
                .OrderByDescending(p => p.Year)
                .ThenByDescending(p => p.Month)
                .FirstOrDefaultAsync();

        if (period is null)
        {
            TempData["Warning"] = "Chưa có kỳ lương để lập báo cáo.";
            return RedirectToAction(nameof(Index));
        }

        int periodId = period.Id;
        int month = period.Month;
        int year = period.Year;

        var q = _db.PayrollEntries
            .Include(e => e.Employee)
            .Where(e => e.PayrollPeriodId == periodId);

        // Lấy dữ liệu chi tiết (không dùng ?. trong expression tree)
        var rows = await q
            .OrderBy(e => e.Employee != null ? (e.Employee.FullName ?? "") : "")
            .Select(e => new PayrollReportRow
            {
                FullName = e.Employee != null ? (e.Employee.FullName ?? "") : "",
                Department = e.Employee != null ? (e.Employee.Department ?? "") : "",
                Title = e.Employee != null ? (e.Employee.Title ?? "") : "",
                BaseSalary = e.Employee != null ? e.Employee.BaseSalary : 0M,
                Bonus = e.Bonus,
                NetPay = e.NetPay
            })
            .ToListAsync();

        var vm = new PayrollReportVM
        {
            PeriodId = periodId,
            Month = month,
            Year = year,
            EmployeeCount = rows.Count,
            TotalPayroll = await q.SumAsync(e => (decimal?)e.NetPay) ?? 0M,
            AvgSalary = rows.Count > 0 ? rows.Average(r => r.NetPay) : 0M,
            Rows = rows
        }; // <— đóng object initializer, có dấu chấm phẩy

        return View(vm);
    }


}
