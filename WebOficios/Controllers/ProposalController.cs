using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Proposal;
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
    public class ProposalsController : ControllerBase
    {
        private readonly ILogger<ProposalsController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Proposal> _proposal;
        private readonly IApplication<Job> _job;
        private readonly IApplication<Worker> _worker;
        private readonly IMapper _mapper;
        private readonly IApplication<Client> _client;
        private readonly IApplication<Notification> _notification;

        public ProposalsController(
    IApplication<Proposal> proposal,
    IApplication<Job> job,
    IApplication<Worker> worker,
    ILogger<ProposalsController> logger,
    IStringService stringService,
    IMapper mapper,
    IApplication<Client> client,
    IApplication<Notification> notification)
        {
            _proposal = proposal;
            _job = job;
            _worker = worker;
            _logger = logger;
            _stringService = stringService;
            _client = client;
            _mapper = mapper;
            _notification = notification;
        }

        private Worker ObtenerTrabajadorAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            return _worker.GetAll().FirstOrDefault(w => w.UserId == userGuid);
        }
        private Client ObtenerClienteAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
            {
                return null;
            }

            return _client.GetAll().FirstOrDefault(c => c.UserId == userGuid);
        }

        [HttpPost]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> Crear(ProposalRequestDto proposalRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }

            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            { return Unauthorized(); }

            Job job = _job.GetById(proposalRequestDto.JobId);

            if (job is null)
            { return NotFound("No se encontró el trabajo."); }

            if (job.Status != JobStatus.Publicado)
            { return BadRequest("No se pueden enviar propuestas a este trabajo."); }

            if (proposalRequestDto.LaborCost < 0 ||
                proposalRequestDto.EstimatedMaterialsCost < 0)
            { return BadRequest("Los costos no pueden ser negativos."); }

            if (string.IsNullOrWhiteSpace(proposalRequestDto.Message))
            { return BadRequest("Debe ingresar un mensaje."); }

            var proposal = _mapper.Map<Proposal>(proposalRequestDto);

            proposal.WorkerId = worker.Id;
            proposal.CreatedDate = DateTime.UtcNow;
            proposal.Status = ProposalStatus.Pendiente;

            _proposal.Save(proposal);

            return Ok(proposal.Id);
        }
        [HttpGet]
        [Route("ById")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult ById(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
                return Unauthorized();

            Proposal proposal = _proposal.GetById(Id.Value);

            if (proposal is null)
                return NotFound();

            if (proposal.WorkerId != worker.Id)
                return Forbid();

            return Ok(_mapper.Map<ProposalResponseDto>(proposal));
        }

        [HttpGet]
        [Route("MisPropuestas")]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> MisPropuestas()
        {
            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            { return Unauthorized(); }

            var propuestas = _proposal.GetAll()
                .Where(p => p.WorkerId == worker.Id);

            return Ok(_mapper.Map<IList<ProposalResponseDto>>(propuestas));
        }

        [HttpDelete]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }

            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            { return Unauthorized(); }

            Proposal proposal = _proposal.GetById(Id.Value);

            if (proposal is null)
            { return NotFound(); }

            if (proposal.WorkerId != worker.Id)
            { return Forbid(); }

            if (proposal.Status != ProposalStatus.Pendiente)
            { return BadRequest("Solo se pueden retirar propuestas pendientes."); }

            proposal.Status = ProposalStatus.Retirada;

            _proposal.Save(proposal);

            return Ok();
        }

        [HttpPut]
        [Authorize(Roles = "Trabajador")]
        public async Task<IActionResult> Editar(int? Id, ProposalRequestDto proposalRequestDto)
        {
            if (!Id.HasValue || !ModelState.IsValid)
            { return BadRequest(); }

            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
            { return Unauthorized(); }

            Proposal proposal = _proposal.GetById(Id.Value);

            if (proposal is null)
            { return NotFound(); }

            if (proposal.WorkerId != worker.Id)
            { return Forbid(); }

            if (proposal.Status != ProposalStatus.Pendiente)
            { return BadRequest("Solo se pueden editar propuestas pendientes."); }

            if (proposalRequestDto.LaborCost < 0 ||
                proposalRequestDto.EstimatedMaterialsCost < 0)
            { return BadRequest("Los costos no pueden ser negativos."); }

            if (string.IsNullOrWhiteSpace(proposalRequestDto.Message))
            { return BadRequest("Debe ingresar un mensaje."); }

            int jobId = proposal.JobId;
            int workerId = proposal.WorkerId;
            DateTime createdDate = proposal.CreatedDate;
            ProposalStatus status = proposal.Status;

            _mapper.Map(proposalRequestDto, proposal);

            proposal.JobId = jobId;
            proposal.WorkerId = workerId;
            proposal.CreatedDate = createdDate;
            proposal.Status = status;

            _proposal.Save(proposal);

            return Ok();
        }
        // Métodos que permiten al cliente consultar las propuestas
        // recibidas por sus trabajos, aceptar una propuesta o rechazarla.
        // Al aceptar una, las demás propuestas pendientes del mismo trabajo
        // se rechazan automáticamente.
        [HttpPut]
        [Route("Aceptar")]
        [Authorize(Roles = "Cliente")]
        public IActionResult Aceptar(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            Client client = ObtenerClienteAutenticado();

            if (client is null)
                return Unauthorized();

            Proposal proposal = _proposal.GetById(Id.Value);

            if (proposal is null)
                return NotFound();

            Job job = _job.GetById(proposal.JobId);

            if (job is null)
                return NotFound("No se encontró el trabajo.");

            if (job.ClientId != client.Id)
                return Forbid();

            if (proposal.Status != ProposalStatus.Pendiente)
                return BadRequest("La propuesta ya fue respondida.");

            if (job.Status != JobStatus.Publicado)
                return BadRequest("El trabajo ya no está disponible para aceptar propuestas.");

            var propuestas = _proposal.GetAll()
                .Where(p => p.JobId == proposal.JobId &&
                            p.Status == ProposalStatus.Pendiente)
                .ToList();

            foreach (var item in propuestas)
            {
                item.ResponseDate = DateTime.UtcNow;

                if (item.Id == proposal.Id)
                {
                    item.Status = ProposalStatus.Aceptada;
                    item.ClientResponse = "El cliente aceptó tu propuesta.";

                    var notificacion = new Notification
                    {
                        WorkerId = item.WorkerId,
                        Title = "¡Propuesta aceptada!",
                        Message = $"El cliente aceptó tu propuesta para el trabajo '{job.Title}'.",
                        CreatedDate = DateTime.UtcNow,
                        IsRead = false
                    };

                    _notification.Save(notificacion);
                }
                else
                {
                    item.Status = ProposalStatus.Rechazada;
                    item.ClientResponse = "El cliente seleccionó otra propuesta.";

                    var notificacion = new Notification
                    {
                        WorkerId = item.WorkerId,
                        Title = "Propuesta rechazada",
                        Message = $"Tu propuesta para el trabajo '{job.Title}' fue rechazada porque el cliente seleccionó otra propuesta.",
                        CreatedDate = DateTime.UtcNow,
                        IsRead = false
                    };

                    _notification.Save(notificacion);
                }

                _proposal.Save(item);
            }

            // Cambiar el estado del trabajo a EnProceso.
            job.Status = JobStatus.EnProceso;

            _job.Save(job);

            return Ok();
        }


        [HttpPut]
        [Route("Rechazar")]
        [Authorize(Roles = "Cliente")]
        public IActionResult Rechazar(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            Client client = ObtenerClienteAutenticado();

            if (client is null)
                return Unauthorized();

            Proposal proposal = _proposal.GetById(Id.Value);

            if (proposal is null)
                return NotFound();

            Job job = _job.GetById(proposal.JobId);

            if (job is null)
                return NotFound("No se encontró el trabajo.");

            if (job.ClientId != client.Id)
                return Forbid();

            if (proposal.Status != ProposalStatus.Pendiente)
                return BadRequest("La propuesta ya fue respondida.");

            proposal.Status = ProposalStatus.Rechazada;
            proposal.ClientResponse = "El cliente rechazó tu propuesta.";
            proposal.ResponseDate = DateTime.UtcNow;

            _proposal.Save(proposal);

            var notificacion = new Notification
            {
                WorkerId = proposal.WorkerId,
                Title = "Propuesta rechazada",
                Message = $"Tu propuesta para el trabajo '{job.Title}' fue rechazada por el cliente.",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notification.Save(notificacion);

            return Ok();
        }


        [HttpGet]
        [Route("PropuestasRecibidas")]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> PropuestasRecibidas()
        {
            Client client = ObtenerClienteAutenticado();

            if (client is null)
            { return Unauthorized(); }

            var trabajos = _job.GetAll()
                .Where(j => j.ClientId == client.Id)
                .Select(j => j.Id);

            var propuestas = _proposal.GetAll()
                .Where(p => trabajos.Contains(p.JobId));

            return Ok(_mapper.Map<IList<ProposalResponseDto>>(propuestas));
        }

    }
}



