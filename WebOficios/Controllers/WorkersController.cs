using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
using Oficios.Applications.Dtos.Worker;
using Oficios.Entities;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly ILogger<WorkersController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Worker> _worker;
        private readonly IMapper _mapper;
        public WorkersController(IApplication<Worker> worker
            , ILogger<WorkersController> logger
            , IStringService stringService
            , IMapper mapper)
        {
            _worker = worker;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            return Ok(_mapper.Map<IList<WorkerResponseDto>>(_worker.GetAll()));
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> ById(int? Id)
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }
            Worker worker = _worker.GetById(Id.Value);
            if (worker is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<WorkerResponseDto>(worker));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(WorkerRequestDto workerRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            var worker = _mapper.Map<Worker>(workerRequestDto);
            _worker.Save(worker);
            return Ok(worker.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(int? Id, WorkerRequestDto workerRequestDto)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Worker workerBack = _worker.GetById(Id.Value);
            if (workerBack is null)
            { return NotFound(); }
            _mapper.Map(workerRequestDto, workerBack);
            _worker.Save(workerBack);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            Worker workerBack = _worker.GetById(Id.Value);
            if (workerBack is null)
            { return NotFound(); }
            _worker.Delete(workerBack.Id);
            return Ok();
        }
    }

}

