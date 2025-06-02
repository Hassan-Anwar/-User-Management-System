
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog;
using UserManagementSystem.API.Repositories;

namespace UserManagementSystem.API.Webhooks
{
    public class WebhookService : IWebhookService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public WebhookService(IUserRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
            _http = new HttpClient();
        }

        public async Task SendLoginWebhookAsync()
        {
            var users = await _repository.GetRecentLoginsAsync(TimeSpan.FromMinutes(30));
            var payload = new
            {
                eventType = "user_logged_in",
                timestamp = DateTime.UtcNow,
                activeUsers = users.Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.LastLoginAt
                })
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var webhookUrl = _config["Webhook:Url"];
                var res = await _http.PostAsync(webhookUrl, content);
                res.EnsureSuccessStatusCode();
                Log.Information("Webhook sent successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to send webhook.");
            }
        }
    }
}
