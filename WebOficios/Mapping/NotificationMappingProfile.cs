using AutoMapper;
using Oficios.Applications.Dtos.Notification;
using Oficios.Entities;

namespace Oficios.WebApi.Mapping
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationResponseDto>();
        }
    }
}
