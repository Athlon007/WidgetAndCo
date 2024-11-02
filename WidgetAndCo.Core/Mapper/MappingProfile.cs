using AutoMapper;
using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserResponseDto>();
    }
}