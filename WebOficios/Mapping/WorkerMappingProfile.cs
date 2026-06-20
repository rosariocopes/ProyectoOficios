using AutoMapper;
using Oficios.Applications.Dtos.Worker;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class WorkerMappingProfile : Profile
    {
        public WorkerMappingProfile()
        {
            CreateMap<Worker, WorkerResponseDto>().
                ForMember(dest => dest.Profession, opt => opt.MapFrom(src => src.Profession.Name));
            CreateMap<WorkerRequestDto, Worker>();
        }
    }
}
