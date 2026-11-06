using AutoMapper;
using Oficios.Applications.Dtos.Review;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<ReviewRequestDto, Review>()
                .ForMember(dest => dest.Comment,
                    opt => opt.MapFrom(src => src.Comment.Trim()))
                .ForMember(dest => dest.CreatedDate,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Job,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Client,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Worker,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Author,
                    opt => opt.Ignore())
                .ForMember(dest => dest.JobId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.ClientId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.WorkerId,
                    opt => opt.Ignore())
                .ForMember(dest => dest.AuthorUserId,
                    opt => opt.Ignore());

            CreateMap<Review, ReviewResponseDto>()
                .ForMember(dest => dest.JobTitle,
                    opt => opt.MapFrom(src => src.Job.Title))
                .ForMember(dest => dest.AuthorName,
                    opt => opt.Ignore())
                .ForMember(dest => dest.RecipientName,
                    opt => opt.Ignore())
                .ForMember(dest => dest.AuthorRole,
                    opt => opt.Ignore());
        }
    }
}
