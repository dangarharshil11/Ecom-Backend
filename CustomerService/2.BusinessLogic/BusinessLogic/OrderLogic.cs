using AutoMapper;
using CustomerService._1.WebAPI.HelperServices.IServices;
using CustomerService._2.BusinessLogic.DTO;
using CustomerService._2.BusinessLogic.IBusinessLogic;
using CustomerService._3.DataAccess.Domains;
using CustomerService._3.DataAccess.IRepositories;
using CustomerService._4.Infrastructure.Constants;
using CustomerService._4.Infrastructure.Models;
using Stripe;
using Stripe.Checkout;
using System.Net.Mail;
using System.Net;
using System.Globalization;
using System.Text;
using CustomerService._3.DataAccess.Repositories;

namespace CustomerService._2.BusinessLogic.BusinessLogic
{
    public class OrderLogic(IOrderRepository orderRepository, IMapper mapper,
        IArchiveRepository archiveRepository, ICartLogic cartLogic,
        IProductService productService, IConfiguration configuration) : IOrderLogic
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IArchiveRepository _archiveRepository = archiveRepository;
        private readonly ICartLogic _cartLogic = cartLogic;
        private readonly IProductService _productService = productService;
        private readonly CommonResponse response = new();
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        public async Task<CommonResponse> CreateOrder(List<CartResponseDTO> cartResponses)
        {
            if (cartResponses.Count > 0)
            {
                double OrderAmount = 0;
                List<OrderDetailsDTO> OrderDetails = [];
                bool isAdded = false;
                string stockStatus = "";
                List<Guid> productIds = [];
                List<int> productCounts = [];

                foreach (CartResponseDTO cartResponse in cartResponses)
                {
                    stockStatus = await _productService.CheckForStock(cartResponse.ProductId, cartResponse.ProductCount);

                    if (string.IsNullOrEmpty(stockStatus))
                    {
                        OrderAmount += (cartResponse.ProductCount * cartResponse.Product?.ProductPrice ?? 0);

                        OrderDetails.Add(new OrderDetailsDTO
                        {
                            ProductId = cartResponse.ProductId,
                            Product = cartResponse.Product,
                            ProductCount = cartResponse.ProductCount,
                            ProductUnitPrice = cartResponse.Product?.ProductPrice ?? 0,
                        });

                        isAdded = await _archiveRepository.AddProduct(_mapper.Map<ProductArchive>(cartResponse.Product));
                        isAdded = isAdded && await _archiveRepository.AddUser(_mapper.Map<UserArchive>(cartResponse.User));

                        productIds.Add(cartResponse.ProductId);
                        productCounts.Add(cartResponse.ProductCount);
                    }
                }

                if (!string.IsNullOrEmpty(stockStatus))
                {
                    response.Message = stockStatus;
                }
                else if (isAdded)
                {
                    OrderDTO orderDto = new()
                    {
                        UserId = cartResponses[0].UserId,
                        User = cartResponses[0].User,
                        OrderAmount = OrderAmount,
                        OrderDetails = OrderDetails,
                        OrderTime = DateTime.Now,
                    };

                    Order order = _mapper.Map<Order>(orderDto);
                    order.OrderTime = DateTime.Now;
                    order.Status = Constants.Status_Pending;
                    order = await _orderRepository.CreateOrder(order);
                    orderDto.Id = order.Id;
                    bool isSuccess = await _productService.UpdateStock(productIds, productCounts);

                    if (isSuccess)
                    {
                        CommonResponse commonResponse = await _cartLogic.EmptyCart(order.UserId);

                        response.IsSuccess = true;
                        response.Data = orderDto;

                        response.Message = commonResponse.IsSuccess ? "Order Created." : "Order Created, But Failed to empty cart." + commonResponse.Message;
                    }
                    else
                    {
                        response.Message = "There was an Error with Stock.";
                    }

                }
                else
                {
                    response.Message = "product is null";
                }
            }
            else
            {
                response.Message = "Cart is Empty.";
            }
            return response;
        }

        public async Task<CommonResponse> CreateStripeSession(StripeRequestDTO stripeRequest)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = stripeRequest.ApprovedUrl,
                CancelUrl = stripeRequest.CancelUrl,
                LineItems = [],
                Mode = "payment",
            };

            if (stripeRequest.Order != null && stripeRequest.Order.Id != null)
            {
                foreach (var item in stripeRequest.Order.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.ProductUnitPrice * 100), // $20.99 -> 2099
                            Currency = "USD",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product?.ProductName
                            }
                        },
                        Quantity = (long)(item.ProductCount),
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                var service = new SessionService();

                Session session = service.Create(options);
                stripeRequest.StripeSessionUrl = session.Url;

                Order? existingOrder = await _orderRepository.GetOrderById((Guid)stripeRequest.Order.Id);

                if (existingOrder != null)
                {
                    Order updatedOrder = existingOrder;
                    updatedOrder.StripeSessionId = session.Id;

                    updatedOrder = await _orderRepository.UpdateOrder(existingOrder, updatedOrder);
                    stripeRequest.Order = _mapper.Map<OrderDTO>(updatedOrder);

                    response.IsSuccess = true;
                    response.Data = stripeRequest;
                }
                else
                {
                    response.Message = "Order Does not exists";
                }
            }
            else
            {
                response.Message = "Order is Empty";
            }
            return response;
        }

        public async Task<CommonResponse> ValidateStripeSession(Guid orderId)
        {
            Order? existingOrder = await _orderRepository.GetOrderById(orderId);

            if (existingOrder != null)
            {
                var service = new SessionService();
                Session session = service.Get(existingOrder.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                Order updatedOrder = existingOrder;
                if (paymentIntent.Status == "succeeded")
                {
                    updatedOrder.PaymentIntentId = paymentIntent.Id;
                    updatedOrder.Status = Constants.Status_Approved;
                    updatedOrder.Note = "Payment Validated";
                    updatedOrder = await _orderRepository.UpdateOrder(existingOrder, updatedOrder);

                    response.Data = _mapper.Map<OrderDTO>(updatedOrder);
                    response.IsSuccess = true;
                    response.Message = "Payment Validated";
                    await SendOrderEmail(updatedOrder);
                }
                else
                {
                    updatedOrder.Note = "Payment Validation failed";
                    _ = await _orderRepository.UpdateOrder(existingOrder, updatedOrder);
                    response.Message = "Payment was not success.";
                }
            }
            else
            {
                response.Message = "Order does not exists.";
            }

            return response;
        }

        public async Task<CommonResponse> GetOrderById(Guid orderId)
        {
            if (Guid.Empty != orderId)
            {
                Order? order = await _orderRepository.GetOrderById(orderId);

                if (order != null)
                {
                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        ProductArchive? productArchive = await _archiveRepository.GetProduct(orderDetail.ProductId);
                        if (productArchive != null)
                        {
                            orderDetail.Product = _mapper.Map<ProductDTO>(productArchive);
                        }
                    }
                    OrderDTO orderDTO = _mapper.Map<OrderDTO>(order);

                    UserArchive? userArchive = await _archiveRepository.GetUser(order.UserId);
                    if (userArchive != null)
                    {
                        orderDTO.User = _mapper.Map<UserDTO>(userArchive);
                    }

                    response.Data = orderDTO;
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Order does not exists";
                }
            }
            else
            {
                response.Message = "OrderId is invalid";
            }
            return response;
        }

        public async Task<CommonResponse> GetOrderByUserId(Guid userId)
        {
            if (Guid.Empty == userId)
            {
                response.Message = "UserId is Invalid";
            }
            else
            {
                List<Order> orders = await _orderRepository.GetOrdersByUserId(userId);
                if (orders.Count > 0)
                {
                    List<OrderDTO> orderDTOs = [];
                    foreach (Order order in orders)
                    {
                        foreach (OrderDetail orderDetail in order.OrderDetails)
                        {
                            ProductArchive? productArchive = await _archiveRepository.GetProduct(orderDetail.ProductId);
                            if (productArchive != null)
                            {
                                orderDetail.Product = _mapper.Map<ProductDTO>(productArchive);
                            }
                        }
                        OrderDTO orderDTO = _mapper.Map<OrderDTO>(order);

                        UserArchive? userArchive = await _archiveRepository.GetUser(order.UserId);
                        if (userArchive != null)
                        {
                            orderDTO.User = _mapper.Map<UserDTO>(userArchive);
                        }
                        orderDTOs.Add(orderDTO);
                    }
                    response.Data = orderDTOs;
                    response.IsSuccess = true;
                }
                else
                {
                    response.IsSuccess = true;
                    response.Message = "No orders are placed yet!.";
                }
            }
            return response;
        }

        public async Task<CommonResponse> GetOrders(FilterModel filter)
        {
            if (filter.Page == null && filter.PageSize == null)
            {
                filter.Page = 1;
                filter.PageSize = 5;
            }

            List<Order> orders = await _orderRepository.GetOrders(filter);
            if (orders.Count > 0)
            {
                List<OrderDTO> orderDTOs = [];
                foreach (Order order in orders)
                {
                    foreach (OrderDetail orderDetail in order.OrderDetails)
                    {
                        ProductArchive? productArchive = await _archiveRepository.GetProduct(orderDetail.ProductId);
                        if (productArchive != null)
                        {
                            orderDetail.Product = _mapper.Map<ProductDTO>(productArchive);
                        }
                    }
                    OrderDTO orderDTO = _mapper.Map<OrderDTO>(order);

                    UserArchive? userArchive = await _archiveRepository.GetUser(order.UserId);
                    if (userArchive != null)
                    {
                        orderDTO.User = _mapper.Map<UserDTO>(userArchive);
                    }
                    orderDTOs.Add(orderDTO);
                }

                response.IsSuccess = true;
                response.Data = new
                {
                    orderDTOs,
                    OrderRepository.TotalRecords,
                };

                return response;
            }
            else
            {
                response.IsSuccess = true;
                response.Message = "No orders are placed yet!.";
            }
            return response;
        }

        public async Task<CommonResponse> UpdateOrder(UpdateOrderRequestDTO request)
        {
            if (Guid.Empty == request.OrderId)
            {
                response.Message = "OrderId is Invalid";
            }
            else
            {
                Order? existingOrder = await _orderRepository.GetOrderById(request.OrderId);
                if (existingOrder != null)
                {
                    Order updatedOrder = existingOrder;
                    updatedOrder.Status = request.Status;
                    updatedOrder.Note = request.Note;

                    await _orderRepository.UpdateOrder(existingOrder, updatedOrder);

                    response.Data = _mapper.Map<OrderDTO>(updatedOrder);
                    response.IsSuccess = true;
                    response.Message = "Order Status updated Successfully";
                }
                else
                {
                    response.Message = "Order does not exists";
                }
            }
            return response;
        }

        private async Task SendOrderEmail(Order order)
        {
            string fromMail = _configuration["SMTP:FromEmail"] ?? "";
            UserArchive? user = await _archiveRepository.GetUser(order.UserId);
            string toMail = user?.Email ?? "";
            string fromPassword = _configuration["SMTP:Password"] ?? "";

            // Read the HTML template from file
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\4.Infrastructure\\EmailTemplates\\NewCustomerOrderEmailTemplate.html");
            string body = System.IO.File.ReadAllText(templatePath);

            body = body
                .Replace("{FirstName}", user?.FirstName)
                .Replace("{LastName}", user?.LastName)
                .Replace("{OrderId}", order.Id.ToString())
                .Replace("{OrderAmount}", order.OrderAmount.ToString("C", new CultureInfo("en-US")))
                .Replace("{OrderTime}", order.OrderTime.ToString("yyyy-MM-dd HH:mm:ss") ?? "Never")
                .Replace("{Status}", order.Status)
                .Replace("{Note}", order.Note ?? "None")
                .Replace("{PaymentIntentId}", order.PaymentIntentId)
                .Replace("{Email}", user?.Email)
                .Replace("{Address}", user?.Address)
                .Replace("{PhoneNumber}", user?.PhoneNumber);

            // Generate the order details table rows dynamically
            StringBuilder orderDetailsHtml = new();
            foreach (OrderDetail detail in order.OrderDetails)
            {
                ProductArchive? product = await _archiveRepository.GetProduct(detail.ProductId);
                string productName = product?.ProductName ?? "Unknown Product";
                string productCount = detail.ProductCount.ToString();
                string productUnitPrice = detail.ProductUnitPrice.ToString("C", new CultureInfo("en-US"));
                string totalPrice = (detail.ProductUnitPrice * detail.ProductCount).ToString("C", new CultureInfo("en-US"));

                orderDetailsHtml.AppendLine($@"
            <tr>
                <td>{productName}</td>
                <td>{productCount}</td>
                <td>{productUnitPrice}</td>
                <td>{totalPrice}</td>
            </tr>");
            }

            body = body.Replace("{#OrderDetails}", orderDetailsHtml.ToString())
                       .Replace("{/OrderDetails}", "");

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = "Payment Success for Order #" + order.Id,
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
