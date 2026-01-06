using PayrollMvc.Data;
using PayrollMvc.Models;
using Microsoft.EntityFrameworkCore;

namespace PayrollMvc.Services
{
    public class PayrollCalculator
    {
        private readonly AppDbContext _db;
        public PayrollCalculator(AppDbContext db) => _db = db;

        public (decimal social, decimal tax, decimal net) CalculateFor(Employee emp, decimal bonus)
        {
            decimal gross = emp.BaseSalary + bonus;
            decimal social = Math.Round(emp.BaseSalary * 0.08m, 0); // 8% BHXH on base
            decimal taxable = Math.Max(0, gross - 11000000); // personal deduction 11m (demo)
            decimal tax = Math.Round(taxable * 0.1m, 0); // single bracket 10% (demo)
            decimal net = gross - social - tax;
            return (social, tax, net);
        }

        public async Task<int> EnsureEntriesForPeriodAsync(int payrollPeriodId)
        {
            var period = await _db.PayrollPeriods.FindAsync(payrollPeriodId);
            if (period == null) return 0;

            var employees = await _db.Employees.AsNoTracking().ToListAsync();
            int created = 0;
            foreach (var emp in employees)
            {
                bool exists = await _db.PayrollEntries.AnyAsync(e => e.EmployeeId == emp.Id && e.PayrollPeriodId == payrollPeriodId);
                if (!exists)
                {
                    var (social, tax, net) = CalculateFor(emp, 0);
                    _db.PayrollEntries.Add(new PayrollEntry
                    {
                        EmployeeId = emp.Id,
                        PayrollPeriodId = payrollPeriodId,
                        Bonus = 0,
                        SocialInsurance = social,
                        IncomeTax = tax,
                        NetPay = net
                    });
                    created++;
                }
            }
            await _db.SaveChangesAsync();
            return created;
        }

        public async Task RecalculatePeriodAsync(int payrollPeriodId)
        {
            var entries = await _db.PayrollEntries
                .Include(e => e.Employee)
                .Where(e => e.PayrollPeriodId == payrollPeriodId)
                .ToListAsync();

            foreach (var e in entries)
            {
                var (social, tax, net) = CalculateFor(e.Employee!, e.Bonus);
                e.SocialInsurance = social;
                e.IncomeTax = tax;
                e.NetPay = net;
            }
            await _db.SaveChangesAsync();
        }
    }
}
