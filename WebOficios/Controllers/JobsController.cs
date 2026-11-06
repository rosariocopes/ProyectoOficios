
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Job;
using Oficios.Entities;
using Oficios.Enums;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Job> _job;
        private readonly IApplication<Client> _client;
        private readonly IApplication<Worker> _worker;
        private readonly IApplication<Proposal> _proposal;
        private readonly IApplication<Notification> _notification;
        private readonly IApplication<Review> _review;
        private readonly IMapper _mapper;

        public JobsController(
            IApplication<Job> job,
            IApplication<Client> client,
            IApplication<Worker> worker,
            IApplication<Proposal> proposal,
            IApplication<Notification> notification,
            IApplication<Review> review,
            ILogger<JobsController> logger,
            IStringService stringService,
            IMapper mapper)
        {
            _job = job;
            _client = client;
            _worker = worker;
            _proposal = proposal;
            _notification = notification;
            _review = review;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }

        private Client? ObtenerClienteAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return null;

            return _client.GetAll()
                .FirstOrDefault(c => c.UserId == userGuid);
        }

        private Worker? ObtenerTrabajadorAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return null;

            return _worker.GetAll()
                .FirstOrDefault(w => w.UserId == userGuid);
        }

        private IList<JobResponseDto> MapearTrabajos(IEnumerable<Job> trabajos)
        {
            var resultado = _mapper.Map<IList<JobResponseDto>>(trabajos);

            var clientes = _client.GetAll().ToList();
            var resenas = _review.GetAll().ToList();

            foreach (var trabajo in resultado)
            {
                var cliente = clientes.FirstOrDefault(c => c.Id == trabajo.ClientId);

                if (cliente == null)
                    continue;

                var resenasCliente = resenas
                    .Where(r => r.ClientId == cliente.Id)
                    .Where(r => r.AuthorUserId != cliente.UserId)
                    .ToList();

                trabajo.AverageScore = resenasCliente.Any()
                    ? Math.Round(resenasCliente.Average(r => r.Score), 2)
                    : 0;

                trabajo.ReviewCount = resenasCliente.Count;
            }

            return resultado;
        }

        [HttpGet]
        [Route("All")]
        public IActionResult All()
        {
            var trabajos = _job.GetAll()
                .OrderByDescending(j => j.CreatedDate)
                .ToList();

            return Ok(MapearTrabajos(trabajos));
        }

        [HttpGet]
        [Route("ById")]
        public IActionResult ById(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            var job = _job.GetById(Id.Value);

            if (job is null)
                return NotFound();

            var resultado = MapearTrabajos(new List<Job> { job }).First();

            return Ok(resultado);
        }

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult Crear([FromBody] JobRequestDto jobRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = ObtenerClienteAutenticado();

            if (client == null)
                return Forbid();

            var job = _mapper.Map<Job>(jobRequestDto);

            job.ClientId = client.Id;
            job.CreatedDate = DateTime.UtcNow;
            job.Status = JobStatus.Publicado;

            _job.Save(job);

            return Ok(job.Id);
        }

        [HttpPut]
        [Authorize(Roles = "Cliente")]
        public IActionResult Editar(int? Id, [FromBody] JobRequestDto jobRequestDto)
        {
            if (!Id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = ObtenerClienteAutenticado();

            if (client == null)
                return Forbid();

            Job jobBack = _job.GetById(Id.Value);

            if (jobBack is null)
                return NotFound();

            if (jobBack.ClientId != client.Id)
                return Forbid();

            if (jobBack.Status != JobStatus.Publicado)
                return BadRequest("Solo se pueden modificar trabajos publicados.");

            var tienePropuestas = _proposal.GetAll()
                .Any(p => p.JobId == jobBack.Id);

            if (tienePropuestas)
            {
                return BadRequest(
                    "No se puede modificar el trabajo porque ya recibió propuestas."
                );
            }

            _mapper.Map(jobRequestDto, jobBack);

            jobBack.ClientId = client.Id;

            _job.Save(jobBack);

            return Ok();
        }

        [HttpDelete]
        [Authorize(Roles = "Cliente")]
        public IActionResult Borrar(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            var client = ObtenerClienteAutenticado();

            if (client == null)
                return Forbid();

            Job jobBack = _job.GetById(Id.Value);

            if (jobBack is null)
                return NotFound();

            if (jobBack.ClientId != client.Id)
                return Forbid();

            var propuestas = _proposal.GetAll()
                .Where(p => p.JobId == jobBack.Id)
                .ToList();

            if (propuestas.Any(p => p.Status == ProposalStatus.Aceptada))
            {
                return BadRequest(
                    "No se puede eliminar el trabajo porque tiene una propuesta aceptada."
                );
            }

            foreach (var propuesta in propuestas)
            {
                if (propuesta.Status == ProposalStatus.Pendiente)
                {
                    propuesta.Status = ProposalStatus.Rechazada;
                    propuesta.ClientResponse = "El cliente eliminó la publicación del trabajo.";
                    propuesta.ResponseDate = DateTime.UtcNow;

                    _proposal.Save(propuesta);
                }
            }

            foreach (var trabajadorId in propuestas
                .Select(p => p.WorkerId)
                .Distinct())
            {
                var notificacion = new Notification
                {
                    WorkerId = trabajadorId,
                    Title = "Trabajo eliminado",
                    Message = $"El trabajo '{jobBack.Title}' al que habías enviado una propuesta fue eliminado por el cliente.",
                    CreatedDate = DateTime.UtcNow,
                    IsRead = false
                };

                _notification.Save(notificacion);
            }

            _job.Delete(jobBack.Id);

            return Ok();
        }

        [HttpGet]
        [Route("MisTrabajos")]
        [Authorize(Roles = "Cliente")]
        public IActionResult MisTrabajos()
        {
            var client = ObtenerClienteAutenticado();

            if (client == null)
                return Forbid();

            var trabajos = _job.GetAll()
                .Where(j => j.ClientId == client.Id &&
                            j.Status != JobStatus.Finalizado)
                .OrderByDescending(j => j.CreatedDate)
                .ToList();

            return Ok(MapearTrabajos(trabajos));
        }

        [HttpGet]
        [Route("HistorialCliente")]
        [Authorize(Roles = "Cliente")]
        public IActionResult HistorialCliente()
        {
            var client = ObtenerClienteAutenticado();

            if (client == null)
                return Forbid();

            var trabajos = _job.GetAll()
                .Where(j => j.ClientId == client.Id &&
                            j.Status == JobStatus.Finalizado)
                .OrderByDescending(j => j.CreatedDate)
                .ToList();

            return Ok(MapearTrabajos(trabajos));
        }

        [HttpGet]
        [Route("HistorialTrabajador")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult HistorialTrabajador()
        {
            var worker = ObtenerTrabajadorAutenticado();

            if (worker == null)
                return Forbid();

            var trabajosIds = _proposal.GetAll()
                .Where(p => p.WorkerId == worker.Id &&
                            p.Status == ProposalStatus.Aceptada)
                .Select(p => p.JobId)
                .Distinct()
                .ToList();

            var trabajos = _job.GetAll()
                .Where(j => trabajosIds.Contains(j.Id) &&
                            j.Status == JobStatus.Finalizado)
                .OrderByDescending(j => j.CreatedDate)
                .ToList();

            return Ok(MapearTrabajos(trabajos));
        }
    }
}
