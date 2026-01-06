using PayrollMvc.Models;

namespace PayrollMvc.Data
{
    public static class AppDbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Tạo DB nếu chưa có
            await context.Database.EnsureCreatedAsync();

            // Thêm nhân viên mẫu
            if (!context.Employees.Any())
            {
                context.Employees.AddRange(
                    new Employee { FullName = "Nguyễn Văn A", Department = "IT", Title = "Dev", BaseSalary = 10000000 },
                    new Employee { FullName = "Trần Thị B", Department = "HR", Title = "HR", BaseSalary = 8000000 }
                );
                await context.SaveChangesAsync();
            }

            // Thêm kỳ lương + entries mẫu
            if (!context.PayrollPeriods.Any())
            {
                var kyLuong = new PayrollPeriod
                {
                    Month = 9,
                    Year = 2025,
                    Notes = "Kỳ lương demo",
                    Entries = new List<PayrollEntry>()
                };

                var emp1 = context.Employees.First();
                var emp2 = context.Employees.Skip(1).First();

                kyLuong.Entries.Add(new PayrollEntry
                {
                    EmployeeId = emp1.Id,
                    Bonus = 2000000,
                    SocialInsurance = 500000,
                    IncomeTax = 300000,
                    NetPay = emp1.BaseSalary + 2000000 - 500000 - 300000 
                });

                kyLuong.Entries.Add(new PayrollEntry
                {
                    EmployeeId = emp2.Id,
                    Bonus = 1000000,
                    SocialInsurance = 400000,
                    IncomeTax = 200000,
                    NetPay = emp2.BaseSalary + 1000000 - 400000 - 200000
                });

                context.PayrollPeriods.Add(kyLuong);
                await context.SaveChangesAsync();
            }
        }
    }
}
