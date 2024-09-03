using AutoMapper;
using ProductService._2.BusinessLogic.DTO;
using ProductService._2.BusinessLogic.IBusinessLogic;
using ProductService._3.DataAccess.Domains;
using ProductService._3.DataAccess.IRepositories;
using ProductService._3.DataAccess.Repositories;
using ProductService._4.Infrastructure.Models;

namespace ProductService._2.BusinessLogic.BusinessLogic
{
    public class ProductLogic(IProductRepository productRepository, IMapper mapper) : IProductLogic
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly CommonResponse response = new();
        private readonly IMapper _mapper = mapper;

        public async Task<CommonResponse> CreateProduct(ProductRequestDTO productRequestDTO)
        {
            if (productRequestDTO == null ||
                string.IsNullOrEmpty(productRequestDTO.ProductName) ||
                string.IsNullOrEmpty(productRequestDTO.ProductDescription) ||
                string.IsNullOrEmpty(productRequestDTO.ProductBrandName) ||
                string.IsNullOrEmpty(productRequestDTO.ImageRepresentationBase64) ||
                productRequestDTO.ProductPrice <= 0 ||
                Guid.Empty == productRequestDTO.CategoryId)
            {
                response.Message = "One or More Fields of ProductRequest is Null or Empty.";
            }
            else
            {
                Product? existingProduct = await _productRepository.GetProductByName(productRequestDTO.ProductName);

                if (existingProduct == null)
                {
                    Product product = _mapper.Map<Product>(productRequestDTO);

                    product = await _productRepository.CreateProduct(product);

                    response.Data = product;
                    response.Message = "Product is Created Successfully";
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Product with this Name already exists!!";
                    response.Data = existingProduct;
                }
            }

            return response;
        }

        public async Task<CommonResponse> GetAllProducts(FilterModel filter)
        {
            if (filter.Page == null && filter.PageSize == null)
            {
                filter.Page = 1;
                filter.PageSize = 5;
            }

            List<Product> products = await _productRepository.GetProductsAsync(filter);

            if (products.Count == 0)
            {
                response.Message = "No Products Found";
            }
            response.IsSuccess = true;
            response.Data = new {
                products,
                ProductRepository.totalRecords,
            };

            return response;
        }

        public async Task<CommonResponse> GetProduct(string? productName, Guid? Id)
        {
            Product? product;
            if (!string.IsNullOrEmpty(productName))
            {
                product = await _productRepository.GetProductByName(productName);
                if (product != null)
                {
                    response.IsSuccess = true;
                    response.Data = product;
                }
                else
                {
                    response.Message = "Product with this name does not exists.";
                }
            }
            else if (Guid.Empty != Id && Id != null)
            {
                product = await _productRepository.GetProductById((Guid)Id);
                if (product != null)
                {
                    response.IsSuccess = true;
                    response.Data = product;
                }
                else
                {
                    response.Message = "Product with this Id does not exists.";
                }
            }
            else
            {
                response.Message = "ProductName or Id is required.";
            }
            return response;
        }

        public async Task<CommonResponse> DeleteProduct(Guid Id)
        {
            if (Guid.Empty != Id)
            {
                Product? product = await _productRepository.GetProductById(Id);
                if (product != null)
                {
                    product = await _productRepository.DeleteProduct(product);

                    response.IsSuccess = true;
                    response.Data = product;
                    response.Message = "Product Deleted Successfully.";
                }
                else
                {
                    response.Message = "Product with this Id does not exists.";
                }
            }
            else
            {
                response.Message = "Product Id is Empty";
            }

            return response;
        }
        
        public async Task<CommonResponse> UpdateProduct(ProductRequestDTO productRequestDTO, Guid productId)
        {
            if (productRequestDTO == null ||
                string.IsNullOrEmpty(productRequestDTO.ProductName) ||
                string.IsNullOrEmpty(productRequestDTO.ProductDescription) ||
                string.IsNullOrEmpty(productRequestDTO.ProductBrandName) ||
                string.IsNullOrEmpty(productRequestDTO.ImageRepresentationBase64) ||
                productRequestDTO.ProductPrice <= 0 ||
                Guid.Empty == productRequestDTO.CategoryId)
            {
                response.Message = "One or More Fields of ProductRequest is Null or Empty.";
            }
            else
            {
                Product? existingProduct = await _productRepository.GetProductById(productId);

                if (existingProduct != null)
                {
                    Product product = _mapper.Map<Product>(productRequestDTO);
                    product.CreatedDate = existingProduct.CreatedDate;
                    product.Id = productId;

                    product = await _productRepository.UpdateProduct(existingProduct, product);

                    response.IsSuccess = true;
                    response.Data = product;
                    response.Message = "Product Updated Successfully.";
                }
                else
                {
                    response.Message = "Product with this Id does not exists.";
                }
            }

            return response;
        }
        public async Task<CommonResponse> GetProductList()
        {
            response.Data = await _productRepository.GetProductList();
            response.IsSuccess = true;

            return response;
        }

        public async Task<CommonResponse> GetProducts(List<Guid> productIds)
        {
            response.Data = await _productRepository.GetProducts(productIds);
            response.IsSuccess = true;

            return response;
        }
    }
}
