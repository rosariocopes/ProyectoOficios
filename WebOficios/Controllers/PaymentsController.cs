using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Payment;
using Oficios.Entities;
using Oficios.Enums;
using OficiosApplications;
using OficiosEntities;

namespace Oficios.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IApplication<Payment> _payment;
        private readonly IApplication<Proposal> _proposal;
        private readonly IApplication<Job> _job;
        private readonly IApplication<Client> _client;
        private readonly IApplication<Worker> _worker;
        private readonly IApplication<PaymentType> _paymentType;
        private readonly IApplication<Notification> _notification;
        private readonly IMapper _mapper;

        public PaymentsController(
            IApplication<Payment> payment,
            IApplication<Proposal> proposal,
            IApplication<Job> job,
            IApplication<Client> client,
            IApplication<Worker> worker,
            IApplication<PaymentType> paymentType,
            IApplication<Notification> notification,
            IMapper mapper)
        {
            _payment = payment;
            _proposal = proposal;
            _job = job;
            _client = client;
            _worker = worker;
            _paymentType = paymentType;
            _notification = notification;
            _mapper = mapper;
        }

        // Obtiene el cliente autenticado
        private Client ObtenerClienteAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return null;

            return _client.GetAll()
                .FirstOrDefault(c => c.UserId == userGuid);
        }

        // Obtiene el trabajador autenticado
        private Worker ObtenerTrabajadorAutenticado()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return null;

            return _worker.GetAll()
                .FirstOrDefault(w => w.UserId == userGuid);
        }

        // Crear un pago
        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public IActionResult Crear([FromBody] PaymentRequestDto paymentRequestDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Client client = ObtenerClienteAutenticado();

            if (client is null)
                return Unauthorized();

            Proposal proposal = _proposal.GetAll()
                .FirstOrDefault(p => p.Id == paymentRequestDto.ProposalId);

            if (proposal is null)
                return NotFound("No se encontró la propuesta.");

            Job job = _job.GetById(proposal.JobId);

            if (job is null)
                return NotFound("No se encontró el trabajo.");

            if (job.ClientId != client.Id)
                return Forbid();

            if (proposal.Status != ProposalStatus.Aceptada)
                return BadRequest("Solo se pueden generar pagos de propuestas aceptadas.");

            PaymentType paymentType = _paymentType.GetById(paymentRequestDto.PaymentTypeId);

            if (paymentType is null)
                return NotFound("No se encontró el tipo de pago.");

            var pagoExistente = _payment.GetAll()
                .FirstOrDefault(p => p.ProposalId == proposal.Id);

            if (pagoExistente is not null)
                return BadRequest("Ya existe un pago registrado para esta propuesta.");

            Payment payment = new Payment
            {
                ProposalId = proposal.Id,
                JobId = proposal.JobId,
                PaymentTypeId = paymentType.Id,
                Amount = proposal.ProposedBudget,
                PaymentDate = DateTime.UtcNow,
                Description = $"Pago del trabajo: {job.Title}",
                Status = PaymentStatus.Pendiente
            };

            _payment.Save(payment);

            return Ok(payment.Id);
        }

        // El cliente informa que realizó el pago
        [HttpPut("InformarPago")]
        [Authorize(Roles = "Cliente")]
        public IActionResult InformarPago([FromQuery] int id)
        {
            Client client = ObtenerClienteAutenticado();

            if (client is null)
                return Unauthorized();

            Payment payment = _payment.GetById(id);

            if (payment is null)
                return NotFound("No se encontró el pago.");

            Job job = _job.GetById(payment.JobId);

            if (job is null)
                return NotFound("No se encontró el trabajo.");

            if (job.ClientId != client.Id)
                return Forbid();

            if (payment.Status != PaymentStatus.Pendiente)
                return BadRequest("El pago no se encuentra pendiente.");

            Proposal proposal = _proposal.GetById(payment.ProposalId);

            if (proposal is null)
                return NotFound("No se encontró la propuesta.");

            Worker worker = _worker.GetById(proposal.WorkerId);

            if (worker is null)
                return NotFound("No se encontró el trabajador.");

            payment.Status = PaymentStatus.InformadoPorCliente;
            payment.PaymentReportedDate = DateTime.UtcNow;

            _payment.Save(payment);

            Notification notification = new Notification
            {
                WorkerId = worker.Id,
                Title = "Pago informado",
                Message = $"El cliente informó el pago del trabajo: {job.Title}. Importe: {payment.Amount:C}. Ingresá para confirmar la recepción.",
                CreatedDate = DateTime.UtcNow,
                IsRead = false
            };

            _notification.Save(notification);

            return Ok("El pago fue informado y se notificó al trabajador.");
        }

        // El trabajador confirma la recepción del pago
        [HttpPut("ConfirmarRecepcion")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult ConfirmarRecepcion([FromQuery] int id)
        {
            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
                return Unauthorized();

            Payment payment = _payment.GetById(id);

            if (payment is null)
                return NotFound("No se encontró el pago.");

            Proposal proposal = _proposal.GetById(payment.ProposalId);

            if (proposal is null)
                return NotFound("No se encontró la propuesta.");

            if (proposal.WorkerId != worker.Id)
                return Forbid();

            if (payment.Status != PaymentStatus.InformadoPorCliente)
                return BadRequest("El cliente todavía no informó este pago.");

            Job job = _job.GetById(payment.JobId);

            if (job is null)
                return NotFound("No se encontró el trabajo.");

            payment.Status = PaymentStatus.Pagado;
            payment.PaymentConfirmedDate = DateTime.UtcNow;

            job.Status = JobStatus.Finalizado;

            _payment.Save(payment);
            _job.Save(job);

            return Ok("Pago confirmado. El trabajo fue finalizado correctamente.");
        }

        // Obtener los pagos del cliente autenticado
        [HttpGet("MisPagos")]
        [Authorize(Roles = "Cliente")]
        public IActionResult MisPagos()
        {
            Client client = ObtenerClienteAutenticado();

            if (client is null)
                return Unauthorized();

            var trabajos = _job.GetAll()
                .Where(j => j.ClientId == client.Id)
                .Select(j => j.Id)
                .ToList();

            var pagos = _payment.GetAll()
                .Where(p => trabajos.Contains(p.JobId))
                .ToList();

            return Ok(_mapper.Map<IList<PaymentResponseDto>>(pagos));
        }

        // Obtener los pagos del trabajador autenticado
        [HttpGet("MisPagosTrabajador")]
        [Authorize(Roles = "Trabajador")]
        public IActionResult MisPagosTrabajador()
        {
            Worker worker = ObtenerTrabajadorAutenticado();

            if (worker is null)
                return Unauthorized();

            var propuestas = _proposal.GetAll()
                .Where(p => p.WorkerId == worker.Id)
                .Select(p => p.Id)
                .ToList();

            var pagos = _payment.GetAll()
                .Where(p => propuestas.Contains(p.ProposalId))
                .ToList();

            return Ok(_mapper.Map<IList<PaymentResponseDto>>(pagos));
        }
    }
}
