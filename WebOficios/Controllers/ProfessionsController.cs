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
    public class ProfessionsController : ControllerBase
    {
        private readonly ILogger<Profession> _logger;
        private readonly IStringService _stringservice;
        private readonly IApplication<Profession> _profession;
        public ProfessionsController(ILogger<Profession> logger, IStringService stringservice, IApplication<Profession> profession)
        {
            _logger = logger;
            _stringservice = stringservice;
            _profession = profession;
        }
        [HttpGet]
        [Route("All")]

        public async Task<IActionResult> All()
        {
            return Ok(_profession.GetAll());
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> ById(int? Id)
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }
            Profession profession = _profession.GetById(Id.Value);
            if (profession is null)
            {
                return NotFound();
            }
            return Ok(profession);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Profession profession)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            _profession.Save(profession);
            return Ok(profession.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int? Id, Profession profession)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Profession professionBack = _profession.GetById(Id.Value);
            if (professionBack is null)
            { return NotFound(); }
            professionBack.Name = profession.Name;
            professionBack.Description = profession.Description;
            _profession.Save(professionBack);
            return Ok(professionBack);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Profession professionBack = _profession.GetById(Id.Value);
            if (professionBack is null)
            { return NotFound(); }
            _profession.Delete(professionBack.Id);
            return Ok();
        }
    }
}
    }
}
