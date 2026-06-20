using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Client;
using Oficios.Applications.Dtos.PaymentType;
using Oficios.Applications.Dtos.PaymentType.Oficios.Applications.Dtos.PaymentType;
using Oficios.Entities;
using Oficios.Services;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentTypesController : ControllerBase
    {
        private readonly ILogger<PaymentTypesController> _logger;
        private readonly IStringService _stringService;
        private readonly IApplication<PaymentType> _paymentType;
        private readonly IMapper _mapper;
        public PaymentTypesController(IApplication<PaymentType> paymentType
            , ILogger<PaymentTypesController> logger
            , IStringService stringService
            , IMapper mapper)
        {
            _paymentType = paymentType;
            _logger = logger;
            _stringService = stringService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> All()
        {
            return Ok(_mapper.Map<IList<PaymentTypeResponseDto>>(_paymentType.GetAll()));
        }

        [HttpGet]
        [Route("ById")]
        public async Task<IActionResult> ById(int? Id)
        {
            if (!Id.HasValue)
            {
                return BadRequest();
            }
            PaymentType paymentType = _paymentType.GetById(Id.Value);
            if (paymentType is null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<PaymentTypeResponseDto>(paymentType));
        }

        [HttpPost]
        public async Task<IActionResult> Crear(PaymentTypeRequestDto paymentTypeRequestDto)
        {
            if (!ModelState.IsValid)
            { return BadRequest(); }
            var paymentType = _mapper.Map<PaymentType>(paymentTypeRequestDto);
            _paymentType.Save(paymentType);
            return Ok(paymentType.Id);
        }

        [HttpPut]
        public async Task<IActionResult> Editar(int? Id, PaymentTypeRequestDto paymentTypeRequestDto)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            if (!ModelState.IsValid)
            { return BadRequest(); }
            PaymentType paymentTypeBack = _paymentType.GetById(Id.Value);
            if (paymentTypeBack is null)
            { return NotFound(); }
            _mapper.Map(paymentTypeRequestDto, paymentTypeBack);
            _paymentType.Save(paymentTypeBack);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Borrar(int? Id)
        {
            if (!Id.HasValue)
            { return BadRequest(); }
            PaymentType paymentTypeBack = _paymentType.GetById(Id.Value);
            if (paymentTypeBack is null)
            { return NotFound(); }
            _paymentType.Delete(paymentTypeBack.Id);
            return Ok();
        }
    }
}

