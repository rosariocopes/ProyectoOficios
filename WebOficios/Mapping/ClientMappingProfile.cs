using AutoMapper;
using Oficios.Applications.Dtos.Client;
using OficiosEntities;

namespace Oficios.WebApi.Mapping
{
    public class ClientMappingProfile : Profile
    {
        public ClientMappingProfile()
        {
            CreateMap<Client, ClientResponseDto>().
                ForMember(dest => dest.DateOfBirth, ori => ori.MapFrom(src => src.DateOfBirth.ToShortDateString()));
            CreateMap<ClientRequestDto, Client>();
        }
    }
}
