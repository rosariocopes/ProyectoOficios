using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
using Oficios.Applications.Dtos.Job;
using Oficios.Entities;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobsController : ControllerBase
    {
        private readonly ILogger<JobsController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Job> _job;
        private readonly IMapper _mapper;
        public JobsController(IApplication<Job> job
            , ILogger<JobsController> logger
            , IStringService stringService
            , IMapper mapper)
        {
            _job = job;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            return Ok(_mapper.Map<IList<JobResponseDto>>(_job.GetAll()));
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> ById(int? Id)
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }
            Job job = _job.GetById(Id.Value);
            if (job is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<JobResponseDto>(job));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(JobRequestDto jobRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            var job = _mapper.Map<Job>(jobRequestDto);
            _job.Save(job);
            return Ok(job.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(int? Id, JobRequestDto jobRequestDto)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Job jobBack = _job.GetById(Id.Value);
            if (jobBack is null)
            { return NotFound(); }
            _mapper.Map(jobRequestDto, jobBack);
            _job.Save(jobBack);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            Job jobBack = _job.GetById(Id.Value);
            if (jobBack is null)
            { return NotFound(); }
            _job.Delete(jobBack.Id);
            return Ok();
        }
    }

}
 
