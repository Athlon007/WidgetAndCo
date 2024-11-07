using AutoMapper;
using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        // User
        CreateMap<User, UserResponseDto>();

        // Product
        CreateMap<Product, ProductResponseDto>();
        CreateMap<ProductRequestDto, Product>();
    }
}