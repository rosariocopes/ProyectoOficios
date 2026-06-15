using Microsoft.AspNetCore.Mvc;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ILogger<Client> _logger;
        private readonly IStringService _stringservice;
        private readonly IApplication<Client> _client;
        public ClientsController(ILogger<Client> logger, IStringService stringservice, IApplication<Client> client)
        {
            _logger = logger;
            _stringservice = stringservice;
            _client = client;
        }
        [HttpGet]
        [Route("All")]

        public async Task <IActionResult> All()
        {
            return Ok(_client.GetAll());
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
            return Ok(client);
        }

        [HttpPost]
        public async Task <IActionResult> Create (Client client)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            _client.Save(client);
            return Ok(client.Id);    
        }

        [HttpPut]
        public async Task<IActionResult> Update(int? Id, Client client)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Client clientBack = _client.GetById(Id.Value);
            if (clientBack is null)
            { return NotFound(); }
            clientBack.Name = client.Name;
            clientBack.LastName = client.LastName;
            clientBack.Email = client.Email;
            clientBack.DateOfBirth = client.DateOfBirth;
            clientBack.Dni = client.Dni;
            clientBack.Phone = client.Phone;
            _client.Save(clientBack);
            return Ok(clientBack);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            Client clientBack = _client.GetById(Id.Value);
            if (clientBack is null)
            { return NotFound(); }
            _client.Delete(clientBack.Id);
            return Ok();
        }
    }
}
