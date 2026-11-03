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
        private readonly IApplication<Review> _review;
        private readonly IMapper _mapper;
        public WorkersController(IApplication<Worker> worker
            , ILogger<WorkersController> logger
            , IApplication<Review> review
            , IStringService stringService
            , IMapper mapper)
        {
            _worker = worker;
            _review = review;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            var trabajadores = _worker.GetAll();

            var reseñas = _review.GetAll();

            var resultado = trabajadores.Select(w => new WorkerResponseDto
            {
                Id = w.Id,
                Name = w.Name,
                LastName = w.LastName,
                Dni = w.Dni,
                Description = w.Description,
                License = w.License,
                Phone = w.Phone,
                Email = w.Email,
                Profession = w.Profession.Name,
                City = w.City,
                ReviewCount = reseñas.Count(r => r.WorkerId == w.Id),
                AverageScore = reseñas
                    .Where(r => r.WorkerId == w.Id)
                    .Any()
                        ? (decimal)reseñas
                            .Where(r => r.WorkerId == w.Id)
                            .Average(r => r.Score)
                        : 0
            });

            return Ok(resultado);
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

