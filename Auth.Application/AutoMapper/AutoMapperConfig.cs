using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Entities;
using AutoMapper;

namespace Auth.Application.AutoMapper;

public sealed class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<User, UserOutput>();
    }
}