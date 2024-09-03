using AutoMapper;
using CustomerService._1.WebAPI.HelperServices.IServices;
using CustomerService._2.BusinessLogic.DTO;
using CustomerService._2.BusinessLogic.IBusinessLogic;
using CustomerService._3.DataAccess.Domains;
using CustomerService._3.DataAccess.IRepositories;
using CustomerService._4.Infrastructure.Models;
using Newtonsoft.Json;
using System.Text;

namespace CustomerService._2.BusinessLogic.BusinessLogic
{
    public class CartLogic(ICartRepository cartRepository, IMapper mapper, IProductService productService, IIdentityService identityService) : ICartLogic
    {
        private readonly ICartRepository _cartRepository = cartRepository;
        private readonly CommonResponse response = new();
        private readonly IMapper _mapper = mapper;
        private readonly IProductService _productService = productService;
        private readonly IIdentityService _identityService = identityService;

        public async Task<CommonResponse> GetByUserId(Guid userId)
        {
            UserDTO? userDTO = await _identityService.GetUser(userId);

            // Check if user exists or not
            if (userDTO != null)
            {
                List<Cart> carts = await _cartRepository.GetByUserId(userId);
                // If Cart is not Empty
                if (carts.Count > 0)
                {
                    List<Guid> productIds = [];
                    foreach (Cart cart in carts)
                    {
                        productIds.Add(cart.ProductId);
                    }
                    // Get Product Details for Product API
                    List<ProductDTO> products = await _productService.GetProducts(productIds);

                    if (products.Count > 0)
                    {
                        List<CartResponseDTO> cartResponse = [];

                        // Include Cart, Product, User Related Info to the Response
                        foreach (Cart cart in carts)
                        {
                            cartResponse.Add(new CartResponseDTO()
                            {
                                Id = cart.Id,
                                UserId = cart.UserId,
                                ProductId = cart.ProductId,
                                ProductCount = cart.ProductCount,
                                Product = products.Find(x => x.Id == cart.ProductId),
                                User = userDTO,
                            });
                        }
                        response.Data = cartResponse;
                    }
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Cart is Empty";
                }
            }
            else
            {
                response.Message = "User Do not Exist";
            }
            return response;
        }

        public async Task<CommonResponse> UpsertCart(CartRequestDTO cartRequestDTO)
        {
            // Validating Data
            if (cartRequestDTO == null ||
                Guid.Empty == cartRequestDTO.UserId ||
                Guid.Empty == cartRequestDTO.ProductId)
            {

                response.Message = "Invalid Request.";
            }
            else
            {
                UserDTO? userDTO = await _identityService.GetUser(cartRequestDTO.UserId);

                // Check if User Exists or not
                if (userDTO != null)
                {
                    Cart cart = _mapper.Map<Cart>(cartRequestDTO);

                    // Get Product Details from Product API
                    List<ProductDTO> products = await _productService.GetProducts([cart.ProductId]);

                    Cart? existingCart = await _cartRepository.GetByProductId(cartRequestDTO.ProductId);
                    // Check if Product Exists or not
                    if (products.Count > 0)
                    {
                        string stockStatus = await _productService.CheckForStock(cartRequestDTO.ProductId, cartRequestDTO.ProductCount + (existingCart?.ProductCount ?? 0));

                        // Check whether reqired quantity is available in the stocks or not
                        if (!string.IsNullOrEmpty(stockStatus))
                        {
                            response.Message = stockStatus;
                        }
                        else
                        {
                            // If cart is empty then add items
                            if (existingCart == null)
                            {
                                if(cart.ProductCount == 0)
                                {
                                    response.Message = "Product count should be greater than 0 to add to the cart.";
                                }
                                else
                                {
                                    cart = await _cartRepository.Create(cart);
                                    response.Message = "Product Added to the Cart";

                                    // Include Cart, Product, User Related Info to the Response
                                    CartResponseDTO cartResponse = new()
                                    {
                                        Id = cart.Id,
                                        UserId = cart.UserId,
                                        ProductId = cart.ProductId,
                                        ProductCount = cart.ProductCount,
                                        Product = products.Find(x => x.Id == cart.ProductId),
                                        User = userDTO
                                    };
                                    response.Data = cartResponse;
                                    response.IsSuccess = true;
                                }
                            }
                            // If cart is already has products then update cart
                            else
                            {
                                if(cart.ProductCount == 0 || existingCart.ProductCount + cart.ProductCount == 0)
                                {
                                    cart = await _cartRepository.Delete(existingCart);
                                    response.Message = "Item Removed from the Cart";
                                }
                                else
                                {
                                    cart.Id = existingCart.Id;
                                    cart.ProductCount += existingCart.ProductCount;
                                    cart = await _cartRepository.Update(existingCart, cart);
                                    response.Message = "Cart Updated Successfully";
                                }
                                CartResponseDTO cartResponse = new()
                                {
                                    Id = cart.Id,
                                    UserId = cart.UserId,
                                    ProductId = cart.ProductId,
                                    ProductCount = cart.ProductCount,
                                    Product = products.Find(x => x.Id == cart.ProductId),
                                    User = userDTO
                                };
                                response.Data = cartResponse;
                                response.IsSuccess = true;
                            }
                        }
                    }
                    else
                    {
                        response.Message = "Product Do not exists.";
                    }

                }
                else
                {
                    response.Message = "User Do not Exist";
                }
            }

            return response;
        }

        public async Task<CommonResponse> DeleteById(Guid Id)
        {
            Cart? cart = await _cartRepository.GetById(Id);

            if (cart != null)
            {
                response.Data = await _cartRepository.Delete(cart);
                response.Message = "Product Removed form Cart Successfully.";
                response.IsSuccess = true;
            }
            else
            {
                response.Message = "Failed to Delete[Items do not exists in cart].";
            }
            return response;
        }

        public async Task<CommonResponse> EmptyCart(Guid userId)
        {
            UserDTO? userDTO = await _identityService.GetUser(userId);

            if (userDTO != null)
            {
                List<Cart> carts = await _cartRepository.GetByUserId(userId);

                if (carts != null)
                {
                    foreach (Cart cart in carts)
                    {
                        await _cartRepository.Delete(cart);
                    }

                    response.Message = "Cart has been Emptied.";
                    response.IsSuccess = true;
                }
                else
                {
                    response.Message = "Cart is already Empty.";
                }
            }
            else
            {
                response.Message = "User Do not Exist";
            }
            return response;
        }       
    }
}
