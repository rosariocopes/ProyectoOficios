using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
   [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ILogger<ClientsController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<Client> _client;
        private readonly IMapper _mapper;
        public ClientsController(IApplication<Client> client
            , ILogger<ClientsController> logger
            , IStringService stringService
            , IMapper mapper)
        {
            _client = client;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            return Ok(_mapper.Map<IList<ClientResponseDto>>(_client.GetAll()));
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> ById(int? Id)
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }
            Client client = _client.GetById(Id.Value);
            if (client is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ClientResponseDto>(client));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(ClientRequestDto clientRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            var client = _mapper.Map<Client>(clientRequestDto);
            _client.Save(client);
            return Ok(client.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(int? Id, ClientRequestDto clientRequestDto)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Client clientBack = _client.GetById(Id.Value);
            if (clientBack is null)
            { return NotFound(); }
            _mapper.Map(clientRequestDto, clientBack);
            _client.Save(clientBack);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            Client clientBack = _client.GetById(Id.Value);
            if (clientBack is null)
            { return NotFound(); }
            _client.Delete(clientBack.Id);
            return Ok();
        }
    }
}