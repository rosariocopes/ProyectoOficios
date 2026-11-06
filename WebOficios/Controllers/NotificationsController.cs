using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Entities;
using Oficios.Applications.Dtos.Notification;
using OficiosApplications;

namespace Oficios.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IApplication<Notification> _notification;
        private readonly IApplication<Worker> _worker;
        private readonly IMapper _mapper;

        public NotificationsController(
            IApplication<Notification> notification,
            IApplication<Worker> worker,
            IMapper mapper)
        {
            _notification = notification;
            _worker = worker;
            _mapper = mapper;
        }

        // Obtiene el trabajador que inició sesión.
        private Worker ObtenerTrabajadorAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            return _worker.GetAll()
                .FirstOrDefault(w => w.UserId == userGuid);
        }

        // Devuelve las notificaciones del trabajador autenticado.
        [HttpGet]
        [Route("MisNotificaciones")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult MisNotificaciones()
        {
            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            {
                return Unauthorized();
            }

            var notificaciones = _notification.GetAll()
                .Where(n => n.WorkerId == worker.Id)
                .OrderByDescending(n => n.CreatedDate)
                .ToList();

            var response = _mapper.Map<IList<NotificationResponseDto>>(notificaciones);

            return Ok(response);
        }

        // Marca una notificación como leída.
        [HttpPut]
        [Route("MarcarComoLeida")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult MarcarComoLeida(int id)
        {
            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            {
                return Unauthorized();
            }

            Notification notificacion = _notification.GetById(id);

            if (notificacion is null)
            {
                return NotFound("No se encontró la notificación.");
            }

            // Verificamos que la notificación pertenezca al trabajador.
            if (notificacion.WorkerId != worker.Id)
            {
                return Forbid();
            }

            notificacion.IsRead = true;

            _notification.Save(notificacion);

            return Ok();
        }
    }
}
