using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
using Oficios.Applications.Dtos.Profession;
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
        private readonly ILogger<ProfessionsController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Profession> _profession;
        private readonly IMapper _mapper;
        public ProfessionsController(IApplication<Profession> profession
            , ILogger<ProfessionsController> logger
            , IStringService stringService
            , IMapper mapper)
        {
            _profession = profession;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            return Ok(_mapper.Map<IList<ProfessionResponseDto>>(_profession.GetAll()));
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
            return Ok(_mapper.Map<ProfessionResponseDto>(profession));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ProfessionRequestDto professionRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            var profession = _mapper.Map<Profession>(professionRequestDto);
            _profession.Save(profession);
            return Ok(profession.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(int? Id, ProfessionRequestDto professionRequestDto)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Profession professionBack = _profession.GetById(Id.Value);
            if (professionBack is null)
            { return NotFound(); }
            _mapper.Map(professionRequestDto, professionBack);
            _profession.Save(professionBack);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            Profession professionBack = _profession.GetById(Id.Value);
            if (professionBack is null)
            { return NotFound(); }
            _profession.Delete(professionBack.Id);
            return Ok();
        }
    }
}



