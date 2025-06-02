
using System.Threading.Tasks;

namespace UserManagementSystem.API.Webhooks
{
    public interface IWebhookService
    {
        Task SendLoginWebhookAsync();
    }
}
