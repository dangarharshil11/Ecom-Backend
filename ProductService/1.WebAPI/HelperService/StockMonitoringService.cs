using Microsoft.EntityFrameworkCore;
using ProductService._3.DataAccess.Context;
using ProductService._3.DataAccess.Domains;
using System.Net;
using System.Net.Mail;

namespace ProductService._1.WebAPI.HelperService
{
    public class StockMonitoringService(IServiceScopeFactory serviceScopeFactory, IConfiguration configuration) : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(10);
        private readonly IConfiguration _configuration = configuration;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckStockLevelsAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckStockLevelsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ProductDbContext>();

            var lowStockItems = await dbContext.StockLevels
                .Where(s => s.CurrentStockItems < s.ThresholdAmount)
                .Include(c => c.Product)
                .ToListAsync(stoppingToken);

            foreach (var stockLevel in lowStockItems)
            {
                Console.WriteLine($"Stock for product {stockLevel.ProductId} is below threshold.");

                if (stockLevel.LastEmailSentDate == null ||
                    (DateTime.UtcNow - stockLevel.LastEmailSentDate.Value).TotalDays >= 1)
                {
                    // Send email and update LastEmailSentDate
                    SendEmail(stockLevel);
                    stockLevel.LastEmailSentDate = DateTime.UtcNow;

                    // Save changes to the database
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
        }

        private void SendEmail(StockLevel stockLevel)
        {
            string fromMail = _configuration["SMTP:FromEmail"] ?? "";
            string toMail = _configuration["SMTP:ToEmail"] ?? "";
            string fromPassword = _configuration["SMTP:Password"] ?? "";

            // Read the HTML template from file
            string templatePath = Path.Combine("/app/4.Infrastructure/EmailTemplates/StockLevelEmailTemplate.html");
            string body = File.ReadAllText(templatePath);

            body = body
                .Replace("{ProductName}", stockLevel.Product?.ProductName)
                .Replace("{ProductId}", stockLevel.ProductId.ToString())
                .Replace("{CurrentStockItems}", stockLevel.CurrentStockItems.ToString())
                .Replace("{ThresholdAmount}", stockLevel.ThresholdAmount.ToString())
                .Replace("{LastOrderDate}", stockLevel.LastOrderDate?.ToString("yyyy-MM-dd") ?? "Never")
                .Replace("{SupplierName}", stockLevel.SupplierName)
                .Replace("{SupplierEmail}", stockLevel.SupplierEmail);

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = "Low Stock Level Reminder for " + stockLevel.Product?.ProductName,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(new MailAddress(toMail));

            SmtpClient smtpClient = new("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true
            };

            smtpClient.Send(message);
        }
    }
}
