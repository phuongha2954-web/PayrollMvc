using System;
using System.Collections.Generic;

namespace PayrollMvc.ViewModels
{
    public class PayslipItem
    {
        public string Label { get; set; } = "";
        public decimal Amount { get; set; }
    }

    public class PayslipViewModel
    {
        // Kỳ lương
        public int PayrollId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        // Nhân viên
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Department { get; set; } = "";
        public string Position { get; set; } = "";

        // Chi tiết
        public List<PayslipItem> Earnings { get; set; } = new();
        public List<PayslipItem> Deductions { get; set; } = new();
        public decimal Gross { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }

        public string Note { get; set; } = "";
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}
