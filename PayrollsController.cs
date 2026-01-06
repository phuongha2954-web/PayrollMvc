using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayrollMvc.Data;
using PayrollMvc.ViewModels;

namespace PayrollMvc.Controllers;

[Authorize]
public class PayrollsController : Controller
{
    private readonly AppDbContext _db;
    public PayrollsController(AppDbContext db) => _db = db;

    // GET: /Payrolls
    public async Task<IActionResult> Index()
    {
        var periods = await _db.PayrollPeriods
            .Include(p => p.Entries)
            .AsNoTracking()
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();

        return View(periods);
    }

    // GET: /Payrolls/Create
    public IActionResult Create() => View();

    // POST: /Payrolls/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int month, int year, string? notes)
    {
        // chống trùng tháng/năm
        if (await _db.PayrollPeriods.AnyAsync(p => p.Month == month && p.Year == year))
        {
            ModelState.AddModelError(string.Empty, $"Kỳ {month}/{year} đã tồn tại.");
            return View();
        }

        var employees = await _db.Employees.AsNoTracking().ToListAsync();
        if (!employees.Any())
        {
            ModelState.AddModelError(string.Empty, "Chưa có nhân viên để tạo kỳ lương.");
            return View();
        }

        var period = new PayrollMvc.Models.PayrollPeriod
        {
            Month = month,
            Year = year,
            Notes = notes,
            Entries = new List<PayrollMvc.Models.PayrollEntry>()
        };

        // tạo entries mặc định cho mọi nhân viên
        foreach (var emp in employees)
        {
            period.Entries.Add(new PayrollMvc.Models.PayrollEntry
            {
                EmployeeId = emp.Id,
                Bonus = 0,
                SocialInsurance = 0,
                IncomeTax = 0,
                // công thức mặc định: có thể tùy biến
                NetPay = emp.BaseSalary
            });
        }

        _db.PayrollPeriods.Add(period);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Payrolls/Details/5  -> danh sách nhân viên trong kỳ
    public async Task<IActionResult> Details(int id)
    {
        var period = await _db.PayrollPeriods
            .Include(p => p.Entries)
                .ThenInclude(e => e.Employee)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == id);

        if (period is null) return NotFound();

        var vm = new PayrollPeriodDetailsVm
        {
            PeriodId = period.Id,
            Month = period.Month,
            Year = period.Year,
            Notes = period.Notes
        };

        var rows = (period.Entries ?? Enumerable.Empty<PayrollMvc.Models.PayrollEntry>())
            .Where(e => e != null && e.Employee != null)
            .OrderBy(e => e!.Employee!.FullName);

        foreach (var e in rows)
        {
            var emp = e!.Employee!;
            vm.Rows.Add(new PayrollPeriodDetailsVm.Row
            {
                EmployeeId = e.EmployeeId,
                FullName = emp.FullName ?? string.Empty,
                Department = emp.Department,
                Title = emp.Title,
                BaseSalary = emp.BaseSalary,
                Bonus = e.Bonus,
                SocialInsurance = e.SocialInsurance,
                IncomeTax = e.IncomeTax,
                NetPay = e.NetPay
            });
        }

        return View(vm);
    }

    // GET: phiếu lương 1 nhân viên
    public async Task<IActionResult> Payslip(int periodId, int employeeId)
    {
        var entry = await _db.PayrollEntries
            .Include(x => x.Employee)
            .Include(x => x.PayrollPeriod)
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PayrollPeriodId == periodId && x.EmployeeId == employeeId);

        if (entry is null || entry.Employee is null || entry.PayrollPeriod is null)
            return NotFound();

        var vm = new PayslipVm
        {
            PeriodId = periodId,
            Month = entry.PayrollPeriod.Month,
            Year = entry.PayrollPeriod.Year,
            Notes = entry.PayrollPeriod.Notes,

            EmployeeId = employeeId,
            EmployeeName = entry.Employee.FullName ?? string.Empty,
            Department = entry.Employee.Department,
            Title = entry.Employee.Title,

            BaseSalary = entry.Employee.BaseSalary,
            Bonus = entry.Bonus,
            SocialInsurance = entry.SocialInsurance,
            IncomeTax = entry.IncomeTax,
            NetPay = entry.NetPay
        };

        return View(vm);
    }
}
