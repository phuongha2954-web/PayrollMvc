using System.Security.Cryptography;
using System.Text;

namespace PayrollMvc.Services
{
    public static class PasswordHasher
    {
        public static string Hash(string input)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes); // ví dụ: "9C..." dạng HEX
        }
        public static bool Verify(string input, string hash) => Hash(input) == hash;
    }
}
