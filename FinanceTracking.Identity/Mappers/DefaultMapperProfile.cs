using AutoMapper;
using FinanceTracking.Identity.DTOs;
using FinanceTracking.Identity.Entities;

namespace FinanceTracking.Identity.Mappers
{
    public sealed class DefaultMapperProfile : Profile
    {
        private void CreateDtoProfile()
        {
            CreateMap<RegisterDto, AppUser>()
                .ReverseMap();

            CreateMap<AppUser, UserDto>();
        }

        public DefaultMapperProfile()
        {
            CreateDtoProfile();
        }
    }
}