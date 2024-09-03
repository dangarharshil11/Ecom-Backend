using AutoMapper;
using CustomerService._2.BusinessLogic.DTO;
using CustomerService._3.DataAccess.Domains;

namespace CustomerService._4.Infrastructure.AutoMapper
{
    public static class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Cart, CartRequestDTO>().ReverseMap();

                config.CreateMap<Order, OrderDTO>().ReverseMap();
                config.CreateMap<OrderDetailsDTO, OrderDetail>().ReverseMap();

                config.CreateMap<ProductArchive, ProductDTO>().ReverseMap();
                config.CreateMap<CategoryArchive, CategoryDTO>().ReverseMap();
                config.CreateMap<UserArchive, UserDTO>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
