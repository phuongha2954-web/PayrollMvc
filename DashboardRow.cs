namespace PayrollMvc.Models
{
    public class DashboardRow
    {
        public string FullName { get; set; } = "";
        public string? Department { get; set; }
        public string? Title { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Bonus { get; set; }
        public decimal NetPay { get; set; }
    }
}
