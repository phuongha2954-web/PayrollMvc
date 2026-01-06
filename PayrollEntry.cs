using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PayrollMvc.Models
{
    public class PayrollEntry
    {
        public int Id { get; set; }

        [Display(Name = "Nhân viên")]
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Display(Name = "Kỳ lương")]
        public int PayrollPeriodId { get; set; }
        public PayrollPeriod? PayrollPeriod { get; set; }

        [Display(Name = "Thưởng")]
        public decimal Bonus { get; set; }

        [Display(Name = "BHXH")]
        public decimal SocialInsurance { get; set; }

        [Display(Name = "Thuế TNCN")]
        public decimal IncomeTax { get; set; }

        [Display(Name = "Thực lĩnh")]
        public decimal NetPay { get; set; }
    }
}
