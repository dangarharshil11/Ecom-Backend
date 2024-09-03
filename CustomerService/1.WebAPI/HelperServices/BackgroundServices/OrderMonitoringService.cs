using CustomerService._3.DataAccess.Context;
using CustomerService._4.Infrastructure.Constants;
using Microsoft.EntityFrameworkCore;
using CustomerService._3.DataAccess.Domains;
using CustomerService._2.BusinessLogic.IBusinessLogic;
using CustomerService._4.Infrastructure.Models;

namespace CustomerService._1.WebAPI.HelperServices.BackgroundServices
{
    public class OrderMonitoringService(IServiceScopeFactory serviceScopeFactory) : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckOrdersAsync(stoppingToken);
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckOrdersAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
            IOrderLogic _orderLogic = scope.ServiceProvider.GetRequiredService<IOrderLogic>();

            List<Order> pendingOrders = await dbContext.Orders
                .Where(s => s.Status == Constants.Status_Pending && s.Note == null && s.PaymentIntentId == null)
                .ToListAsync(stoppingToken);

            Console.WriteLine("Pament Validation Started");
            foreach (Order order in pendingOrders)
            {
                // Validate Payment
                CommonResponse response = await _orderLogic.ValidateStripeSession(order.Id);

                if(response.IsSuccess && response.Message == "Payment Validated")
                {
                    Console.WriteLine($"Payment Validated for Order {order.Id}");
                }
                else
                {
                    Console.WriteLine($"Payment Validation Failed for Order {order.Id}");
                }

            }
            Console.WriteLine("Pament Validation Finished");
        }

    }
}