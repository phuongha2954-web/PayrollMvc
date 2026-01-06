using System.ComponentModel.DataAnnotations;

namespace PayrollMvc.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Phòng ban")]
        public string? Department { get; set; }

        [StringLength(50)]
        [Display(Name = "Chức vụ")]
        public string? Title { get; set; }

        [Display(Name = "Lương cơ bản")]
        [Range(0, 1000000000)]
        public decimal BaseSalary { get; set; }
    }
}
