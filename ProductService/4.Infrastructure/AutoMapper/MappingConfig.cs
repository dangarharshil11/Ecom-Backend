using AutoMapper;
using ProductService._2.BusinessLogic.DTO;
using ProductService._3.DataAccess.Domains;

namespace ProductService._4.Infrastructure.AutoMapper
{
    public static class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<Product, ProductRequestDTO>().ReverseMap();
                config.CreateMap<Category, CategoryRequestDTO>().ReverseMap();
                config.CreateMap<StockLevel, StockLevelRequestDTO>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}
