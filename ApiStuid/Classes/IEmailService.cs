using System.Threading.Tasks;

namespace ApiStuid.Classes
{
    public interface IEmailService
    {
        Task SendPasswordResetCodeAsync(string email, int resetCode);
    }
}
