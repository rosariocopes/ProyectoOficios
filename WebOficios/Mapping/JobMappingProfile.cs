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
            CreateMap<Job, JobResponseDto>().
                ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client.Name));
            CreateMap<JobRequestDto, Job>();
        }
    }
    
}
