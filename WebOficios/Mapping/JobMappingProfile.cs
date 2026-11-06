using AutoMapper;
using Oficios.Applications.Dtos.Job;
using Oficios.Applications.Dtos.Worker;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<Job, JobResponseDto>()
                .ForMember(dest => dest.ClientId,
                    opt => opt.MapFrom(src => src.ClientId))
                .ForMember(dest => dest.Client,
                    opt => opt.MapFrom(src =>
                        src.Client.Name + " " + src.Client.LastName))
                .ForMember(dest => dest.Profession,
                    opt => opt.MapFrom(src => src.Profession.Name));

            CreateMap<JobRequestDto, Job>();
        }

    }
}
