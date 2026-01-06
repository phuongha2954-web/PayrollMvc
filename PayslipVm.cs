// ViewModels/PayslipVm.cs
using PayrollMvc.Models;

namespace PayrollMvc.ViewModels
{
    public class PayslipVm
    {
        public int PeriodId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Notes { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = "";
        public string? Department { get; set; }
        public string? Title { get; set; }

        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal SocialInsurance { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetPay { get; set; }
    }
}
