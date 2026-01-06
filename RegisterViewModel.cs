using System.ComponentModel.DataAnnotations;

namespace PayrollMvc.ViewModels
{
    public class RegisterViewModel
    {
        [Required, Display(Name = "Tên đăng nhập")]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password), Display(Name = "Mật khẩu")]
        public string Password { get; set; } = string.Empty;

        [StringLength(100), Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;
    }
}
