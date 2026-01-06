using System.ComponentModel.DataAnnotations;

namespace PayrollMvc.Models
{
    public class PayrollPeriod
    {
        public int Id { get; set; }

        [Range(1,12)]
        [Display(Name = "Tháng")]
        public int Month { get; set; }

        [Range(2000, 2100)]
        [Display(Name = "Năm")]
        public int Year { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        public ICollection<PayrollEntry> Entries { get; set; } = new List<PayrollEntry>();
    }
}
