using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Entities;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkersController : ControllerBase
    {
        private readonly ILogger<Worker> _logger;
        private readonly IStringService _stringservice;
        private readonly IApplication<Worker> _worker;
        public WorkersController(ILogger<Worker> logger, IStringService stringservice, IApplication<Worker> worker)
        {
            _logger = logger;
            _stringservice = stringservice;
            _worker = worker;
        }
        [HttpGet]
        [Route("All")]

        public async Task<IActionResult> All()
        {
            return Ok(_worker.GetAll());
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
            return Ok(worker);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Worker worker)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            _worker.Save(worker);
            return Ok(worker.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int? Id, Worker worker)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Worker workerBack = _worker.GetById(Id.Value);
            if (workerBack is null)
            { return NotFound(); }
            workerBack.Name = worker.Name;
            workerBack.LastName = worker.LastName;
            workerBack.Email = worker.Email;
            workerBack.Dni = worker.Dni;
            workerBack.License = worker.License;
            workerBack.Description = worker.Description;
            workerBack.Phone = worker.Phone;
            workerBack.Profession = worker.Profession;
            _worker.Save(workerBack);
            return Ok(workerBack);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Worker workerBack = _worker.GetById(Id.Value);
            if (workerBack is null)
            { return NotFound(); }
            _worker.Delete(workerBack.Id);
            return Ok();
        }
    }
}

