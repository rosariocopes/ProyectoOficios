using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
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
        private readonly IMapper _mapper;

        public JobsController(
            IApplication<Job> job,
            IApplication<Client> client,
            ILogger<JobsController> logger,
            IStringService stringService,
            IMapper mapper)
        {
            _job = job;
            _client = client;
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

 
        [HttpGet]
        [Route("All")]
        public IActionResult All()
        {
            return Ok(
                _mapper.Map<IList<JobResponseDto>>(_job.GetAll())
            );
        }
        [HttpGet]
        [Route("ById")]
        public IActionResult ById(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            Job job = _job.GetById(Id.Value);

            if (job is null)
                return NotFound();

            return Ok(_mapper.Map<JobResponseDto>(job));
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
                .Where(j => j.ClientId == client.Id);

            return Ok(_mapper.Map<IList<JobResponseDto>>(trabajos));
        }


    }
}

