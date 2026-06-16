using AutoMapper;
using Oficios.Applications.Dtos.Profession;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class ProfessionMappingProfile : Profile
    {
        public ProfessionMappingProfile()
            {
                CreateMap<Profession, ProfessionResponseDto>();
                CreateMap<ProfessionRequestDto, Profession>();
            }
        }
    }

