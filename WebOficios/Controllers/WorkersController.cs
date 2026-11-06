using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Worker;
using Oficios.Entities;
using Oficios.Services;
using OficiosApplications;

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

        public WorkersController(
            IApplication<Worker> worker,
            ILogger<WorkersController> logger,
            IApplication<Review> review,
            IStringService stringService,
            IMapper mapper)
        {
            _worker = worker;
            _review = review;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }

        private WorkerResponseDto MapearTrabajador(Worker worker)
        {
            var resenasRecibidas = _review.GetAll()
                .Where(r => r.WorkerId == worker.Id)
                .Where(r => r.AuthorUserId != worker.UserId)
                .ToList();

            return new WorkerResponseDto
            {
                Id = worker.Id,
                Name = worker.Name,
                LastName = worker.LastName,
                Dni = worker.Dni,
                Description = worker.Description,
                License = worker.License,
                Phone = worker.Phone,
                Email = worker.Email,
                Profession = worker.Profession.Name,
                City = worker.City,
                ReviewCount = resenasRecibidas.Count,
                AverageScore = resenasRecibidas.Any()
                    ? (decimal)resenasRecibidas.Average(r => r.Score)
                    : 0
            };
        }

        [HttpGet]
        [Route("All")]
        public IActionResult All()
        {
            var trabajadores = _worker.GetAll();

            var resultado = trabajadores
                .Select(MapearTrabajador)
                .ToList();

            return Ok(resultado);
        }

        [HttpGet]
        [Route("ById")]
        public IActionResult ById(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            var worker = _worker.GetById(Id.Value);

            if (worker is null)
                return NotFound();

            return Ok(MapearTrabajador(worker));
        }

        [HttpPost]
        public IActionResult Crear(WorkerRequestDto workerRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var worker = _mapper.Map<Worker>(workerRequestDto);

            _worker.Save(worker);

            return Ok(worker.Id);
        }

        [HttpPut]
        public IActionResult Editar(int? Id, WorkerRequestDto workerRequestDto)
        {
            if (!Id.HasValue)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest();

            Worker workerBack = _worker.GetById(Id.Value);

            if (workerBack is null)
                return NotFound();

            _mapper.Map(workerRequestDto, workerBack);

            _worker.Save(workerBack);

            return Ok();
        }

        [HttpDelete]
        public IActionResult Borrar(int? Id)
        {
            if (!Id.HasValue)
                return BadRequest();

            Worker workerBack = _worker.GetById(Id.Value);

            if (workerBack is null)
                return NotFound();

            _worker.Delete(workerBack.Id);

            return Ok();
        }
    }
}
