using AutoMapper;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using ProductService._3.DataAccess.Repositories;
using ProductService._4.Infrastructure.Models;
using System.Net.Mail;
using System.Net;

namespace ProductService._2.BusinessLogic.BusinessLogic
{
    public class StockLevelLogic(IStockLevelRepository stockLevelRepository, IMapper mapper, IProductRepository productRepository, IConfiguration configuration) : IStockLevelLogic
    {
        private readonly IStockLevelRepository _stockLevelRepository = stockLevelRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly CommonResponse response = new();
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        public async Task<CommonResponse> CreateStockLevel(StockLevelRequestDTO stockLevelRequestDTO)
        {
            if (stockLevelRequestDTO == null ||
                string.IsNullOrEmpty(stockLevelRequestDTO.SupplierEmail) ||
                string.IsNullOrEmpty(stockLevelRequestDTO.SupplierName) ||
                string.IsNullOrEmpty(stockLevelRequestDTO.ProductId.ToString()) ||
                stockLevelRequestDTO.CurrentStockItems < 0 ||
                stockLevelRequestDTO.ThresholdAmount < 1)
            {
                response.Message = "One or More Fields of StockLevelRequest is Null or Empty or Invalid.";
            }
            else
            {
                StockLevel? existingStockLevel = await _stockLevelRepository.GetStockLevelByProductId(stockLevelRequestDTO.ProductId);

                if (existingStockLevel == null)
                {
                    StockLevel stockLevel = _mapper.Map<StockLevel>(stockLevelRequestDTO);

                    Product? product = await _productRepository.GetProductById(stockLevelRequestDTO.ProductId);

                    if(product == null)
                    {
                        response.Message = "Product does not exists.";
                    }
                    else
                    {
                        stockLevel = await _stockLevelRepository.CreateStockLevel(stockLevel);
                        stockLevel.Product = product;

                        response.Data = stockLevel;
                        response.Message = "StockLevel is Created Successfully";
                        response.IsSuccess = true;
                    }
                }
                else
                {
                    response.Message = "StockLevel with this Product already exists!!";
                    response.Data = existingStockLevel;
                }
            }

            return response;
        }

        public async Task<CommonResponse> GetAllStockLevels(FilterModel filter)
        {
            if (filter.Page == null && filter.PageSize == null)
            {
                filter.Page = 1;
                filter.PageSize = 5;
            }

            List<StockLevel> stockLevels = await _stockLevelRepository.GetStockLevelsAsync(filter);

            if (stockLevels.Count == 0)
            {
                response.Message = "No StockLevels Found";
            }
            response.IsSuccess = true;
            response.Data = new
            {
                stockLevels,
                StockLevelRepository.totalRecords,
            };

            return response;
        }

        public async Task<CommonResponse> GetStockLevel(Guid Id)
        {
            StockLevel? stockLevel;
            if (Guid.Empty != Id)
            {
                stockLevel = await _stockLevelRepository.GetStockLevelById(Id);
                if (stockLevel != null)
                {
                    response.IsSuccess = true;
                    response.Data = stockLevel;
                }
                else
                {
                    stockLevel = await _stockLevelRepository.GetStockLevelByProductId(Id);
                    if (stockLevel != null)
                    {
                        response.IsSuccess = true;
                        response.Data = stockLevel;
                    }
                    else
                    {
                        response.Message = "StockLevel does not exists.";
                    }
                }
            }
            else
            {
                response.Message = "StockLevelId or ProductId is required.";
            }
            return response;
        }

        public async Task<CommonResponse> DeleteStockLevel(Guid Id)
        {
            if (Guid.Empty != Id)
            {
                StockLevel? stockLevel = await _stockLevelRepository.GetStockLevelById(Id);

                if (stockLevel != null)
                {
                    Product? product = await _productRepository.GetProductById(stockLevel.ProductId);
                    if (product != null)
                    {
                        response.Message = "Remove Product before Deleting StockLevel.";
                    }
                    else
                    {
                        stockLevel = await _stockLevelRepository.DeleteStockLevel(stockLevel);

                        response.IsSuccess = true;
                        response.Data = stockLevel;
                        response.Message = "StockLevel Deleted Successfully.";
                    }
                }
                else
                {
                    response.Message = "StockLevel with this Id does not exists.";
                }
            }
            else
            {
                response.Message = "StockLevel Id is Empty";
            }

            return response;
        }

        public async Task<CommonResponse> UpdateStockLevel(StockLevelRequestDTO stockLevelRequestDTO, Guid stockLevelId)
        {
            if (stockLevelRequestDTO == null ||
                string.IsNullOrEmpty(stockLevelRequestDTO.SupplierEmail) ||
                string.IsNullOrEmpty(stockLevelRequestDTO.SupplierName) ||
                string.IsNullOrEmpty(stockLevelRequestDTO.ProductId.ToString()) ||
                stockLevelRequestDTO.CurrentStockItems < 0 ||
                stockLevelRequestDTO.ThresholdAmount < 1)
            {
                response.Message = "One or More Fields of StockLevelRequest is Null or Empty or Invalid.";
            }
            else
            {
                StockLevel? existingStockLevel = await _stockLevelRepository.GetStockLevelById(stockLevelId);

                if (existingStockLevel != null)
                {
                    StockLevel stockLevel = _mapper.Map<StockLevel>(stockLevelRequestDTO);
                    stockLevel.LastEmailSentDate = existingStockLevel.LastEmailSentDate;
                    stockLevel.Id = stockLevelId;

                    if(stockLevelRequestDTO.LastOrderDate != existingStockLevel.LastOrderDate)
                    {
                        existingStockLevel.LastOrderDate = stockLevelRequestDTO.LastOrderDate;
                        int quantity = stockLevelRequestDTO.CurrentStockItems - existingStockLevel.CurrentStockItems;
                        SendOrderEmail(existingStockLevel, quantity);
                    }

                    stockLevel = await _stockLevelRepository.UpdateStockLevel(existingStockLevel, stockLevel);

                    response.IsSuccess = true;
                    response.Data = stockLevel;
                    response.Message = "StockLevel Updated Successfully.";
                }
                else
                {
                    response.Message = "StockLevel with this Id does not exists.";
                }
            }

            return response;
        }

        public async Task<CommonResponse> GetStockCount(Guid productId)
        {
            StockLevel? stockLevel = await _stockLevelRepository.GetStockLevelByProductId(productId);

            if(stockLevel == null) 
            {
                response.Message = "Product are not available in stock.";
            }
            else
            {
                response.IsSuccess = true;
                response.Data = stockLevel.CurrentStockItems;
            }
            return response;
        }

        public async Task<CommonResponse> UpdateStockCount(List<Guid> productIds, List<int> productCounts)
        {
            for(int i = 0; i < productIds.Count; i++)
            {
                if (string.IsNullOrEmpty(productIds[i].ToString()) ||
                    productCounts[i] < 0)
                {
                    response.Message = "ProductId or count is Invalid";
                }
                else
                {
                    StockLevel? existingStockLevel = await _stockLevelRepository.GetStockLevelByProductId(productIds[i]);

                    if (existingStockLevel != null)
                    {
                        StockLevel stockLevel = existingStockLevel;
                        stockLevel.CurrentStockItems -= productCounts[i];

                        stockLevel = await _stockLevelRepository.UpdateStockLevel(existingStockLevel, stockLevel);

                        response.IsSuccess = true;
                        response.Data = stockLevel;
                        response.Message = "StockLevel Updated Successfully.";
                    }
                    else
                    {
                        response.Message = "StockLevel with this Id does not exists.";
                    }
                }

            }
            return response;
        }

        private void SendOrderEmail(StockLevel stockLevel, int quantity)
        {
            string fromMail = _configuration["SMTP:FromEmail"] ?? "";
            string toMail = stockLevel.SupplierEmail;
            string fromPassword = _configuration["SMTP:Password"] ?? "";

            // Read the HTML template from file
            string templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\4.Infrastructure\\EmailTemplates\\NewOrderEmailTemplate.html");
            string body = File.ReadAllText(templatePath);

            body = body
                .Replace("{SupplierName}", stockLevel.SupplierName)
                .Replace("{ProductName}", stockLevel.Product?.ProductName)
                .Replace("{ProductId}", stockLevel.ProductId.ToString())
                .Replace("{Quantity}", quantity.ToString())
                .Replace("{OrderDate}", stockLevel.LastOrderDate?.ToString("yyyy-MM-dd") ?? "Never");

            MailMessage message = new()
            {
                From = new MailAddress(fromMail),
                Subject = "New Order Placed for " + stockLevel.Product?.ProductName,
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
