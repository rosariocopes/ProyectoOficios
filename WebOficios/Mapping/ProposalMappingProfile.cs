using AutoMapper;
using Oficios.Applications.Dtos.Proposal;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class ProposalMappingProfile : Profile
    {
        public ProposalMappingProfile()
        {
            CreateMap<Proposal, ProposalResponseDto>()
                .ForMember(dest => dest.WorkerName,
                    opt => opt.MapFrom(src =>
                        src.Worker.Name + " " + src.Worker.LastName));
            CreateMap<ProposalRequestDto, Proposal>()
                .ForMember(dest => dest.ProposedBudget,
                    opt => opt.MapFrom(src =>
                        src.LaborCost + src.EstimatedMaterialsCost));
            CreateMap<Proposal, ProposalResponseDto>()
                 .ForMember(dest => dest.JobTitle,
                    opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.JobStatus,
                   opt => opt.MapFrom(src => src.Job.Status));
        }
    }
}
