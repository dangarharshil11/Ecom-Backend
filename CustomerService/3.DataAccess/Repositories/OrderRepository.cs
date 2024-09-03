using CustomerService._3.DataAccess.Context;
using CustomerService._3.DataAccess.Domains;
using CustomerService._3.DataAccess.IRepositories;
using CustomerService._4.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CustomerService._3.DataAccess.Repositories
{
    public class OrderRepository(CustomerDbContext dbContext) : IOrderRepository
    { 
        private readonly CustomerDbContext _dbContext = dbContext;
        public static int TotalRecords { get; set; } = 0;

        public async Task<Order> CreateOrder(Order order)
        {
            order = (await _dbContext.Orders.AddAsync(order)).Entity;
            await _dbContext.SaveChangesAsync();

            return order;
        }

        public async Task<Order> UpdateOrder(Order existingOrder, Order updatedOrder)
        {
            _dbContext.Entry(existingOrder).CurrentValues.SetValues(updatedOrder);
            await _dbContext.SaveChangesAsync();
            return updatedOrder;
        }

        public async Task<Order?> GetOrderById(Guid Id)
        {
            return await _dbContext.Orders.Include(x => x.OrderDetails).FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<Order>> GetOrdersByUserId(Guid userId)
        {
            return await _dbContext.Orders.Include(x => x.OrderDetails).Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<Order>> GetOrders(FilterModel filter)
        {
            IQueryable<Order> query = _dbContext.Orders.Include(x => x.OrderDetails);

            TotalRecords = query.Count();

            // Apply filtering
            if (!string.IsNullOrEmpty(filter.SearchText) && !string.IsNullOrEmpty(filter.SearchColumn))
            {
                query = query.Where(p => EF.Property<string>(p, filter.SearchColumn).Contains(filter.SearchText));
            }

            // Apply sorting
            if (!string.IsNullOrEmpty(filter.SortByColumn))
            {
                var parameter = Expression.Parameter(typeof(Order), "o");
                var property = Expression.Property(parameter, filter.SortByColumn);
                var sortExpression = Expression.Lambda<Func<Order, object>>(Expression.Convert(property, typeof(object)), parameter);

                query = filter.SortOrder?.ToLower() == "descending"
                    ? query.OrderByDescending(sortExpression)
                    : query.OrderBy(sortExpression);
            }

            // Apply pagination
            var skip = ((filter.Page ?? 1) - 1) * filter.PageSize ?? 5;
            var pagedResult = await query.Skip(skip).Take(filter.PageSize ?? 5).ToListAsync();

            return pagedResult;
        }
    }
}
