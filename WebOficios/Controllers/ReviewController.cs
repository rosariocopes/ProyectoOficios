
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oficios.Applications.Dtos.Review;
using Oficios.Entities;
using Oficios.Enums;
using OficiosApplications;
using OficiosEntities;
using System.Security.Claims;

namespace Oficios.WebApi.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IApplication<Review> _review;
        private readonly IApplication<Job> _job;
        private readonly IApplication<Client> _client;
        private readonly IApplication<Worker> _worker;
        private readonly IApplication<Proposal> _proposal;
        private readonly IApplication<Payment> _payment;
        private readonly IMapper _mapper;

        public ReviewsController(
            IApplication<Review> review,
            IApplication<Job> job,
            IApplication<Client> client,
            IApplication<Worker> worker,
            IApplication<Proposal> proposal,
            IApplication<Payment> payment,
            IMapper mapper)
        {
            _review = review;
            _job = job;
            _client = client;
            _worker = worker;
            _proposal = proposal;
            _payment = payment;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] ReviewRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var job = _job.GetById(dto.JobId);

            if (job == null)
                return NotFound("El trabajo no existe.");

            if (job.Status != JobStatus.Finalizado)
                return BadRequest("Solo se pueden realizar reseñas de trabajos finalizados.");

            var propuesta = _proposal.GetAll()
                .FirstOrDefault(p =>
                    p.JobId == dto.JobId &&
                    p.Status == ProposalStatus.Aceptada);

            if (propuesta == null)
                return BadRequest("No se encontró una propuesta aceptada para este trabajo.");

            var pagoConfirmado = _payment.GetAll()
                .Any(p =>
                    p.JobId == dto.JobId &&
                    p.ProposalId == propuesta.Id &&
                    p.Status == PaymentStatus.Pagado);

            if (!pagoConfirmado)
                return BadRequest("Solo se pueden realizar reseñas cuando el pago esté confirmado.");

            var client = _client.GetById(job.ClientId);
            var worker = _worker.GetById(propuesta.WorkerId);

            if (client == null || worker == null)
                return BadRequest("No se encontraron los participantes del trabajo.");

            var esCliente = client.UserId == userGuid;
            var esTrabajador = worker.UserId == userGuid;

            if (!esCliente && !esTrabajador)
                return Forbid();

            var yaReseno = _review.GetAll()
                .Any(r =>
                    r.JobId == dto.JobId &&
                    r.AuthorUserId == userGuid);

            if (yaReseno)
                return BadRequest("Ya realizaste una reseña para este trabajo.");

            var review = _mapper.Map<Review>(dto);

            review.JobId = job.Id;
            review.ClientId = client.Id;
            review.WorkerId = worker.Id;
            review.AuthorUserId = userGuid;
            review.CreatedDate = DateTime.UtcNow;

            _review.Save(review);

            return Ok(new
            {
                mensaje = "La reseña se creó correctamente."
            });
        }

        [HttpGet("PorTrabajador/{workerId}")]
        public IActionResult ObtenerPorTrabajador(int workerId)
        {
            var worker = _worker.GetById(workerId);

            if (worker == null)
                return NotFound("No se encontró el trabajador.");

            var reviews = _review.GetAll()
                .Where(r => r.WorkerId == workerId)
                .Where(r => r.AuthorUserId != worker.UserId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            var resultado = reviews.Select(MapearRespuesta).ToList();

            return Ok(resultado);
        }

        [HttpGet("PorCliente/{clientId}")]
        public IActionResult ObtenerPorCliente(int clientId)
        {
            var client = _client.GetById(clientId);

            if (client == null)
                return NotFound("No se encontró el cliente.");

            var reviews = _review.GetAll()
                .Where(r => r.ClientId == clientId)
                .Where(r => r.AuthorUserId != client.UserId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            var resultado = reviews.Select(MapearRespuesta).ToList();

            return Ok(resultado);
        }

        [HttpGet("PorTrabajo/{jobId}")]
        public IActionResult ObtenerPorTrabajo(int jobId)
        {
            var reviews = _review.GetAll()
                .Where(r => r.JobId == jobId)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            var resultado = reviews.Select(MapearRespuesta).ToList();

            return Ok(resultado);
        }

        [HttpGet("MisResenas")]
        public IActionResult ObtenerMisResenas()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var reviews = _review.GetAll()
                .Where(r => r.AuthorUserId == userGuid)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();

            var resultado = reviews.Select(MapearRespuesta).ToList();

            return Ok(resultado);
        }

        [HttpGet("PromedioTrabajador/{workerId}")]
        public IActionResult ObtenerPromedioTrabajador(int workerId)
        {
            var worker = _worker.GetById(workerId);

            if (worker == null)
                return NotFound("No se encontró el trabajador.");

            var reviews = _review.GetAll()
                .Where(r => r.WorkerId == workerId)
                .Where(r => r.AuthorUserId != worker.UserId)
                .ToList();

            if (!reviews.Any())
            {
                return Ok(new
                {
                    promedio = 0,
                    cantidad = 0
                });
            }

            return Ok(new
            {
                promedio = Math.Round(reviews.Average(r => r.Score), 2),
                cantidad = reviews.Count
            });
        }

        [HttpGet("Recibidas")]
        public IActionResult ObtenerResenasRecibidas()
        {
            var userId = User.FindFirst("Id")?.Value;

            if (!Guid.TryParse(userId, out var userGuid))
                return Unauthorized();

            var role = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role || c.Type == "role")?.Value;

            List<Review> reviews;

            if (role == "Trabajador")
            {
                var worker = _worker.GetAll()
                    .FirstOrDefault(w => w.UserId == userGuid);

                if (worker == null)
                    return NotFound("No se encontró el perfil del trabajador.");

                reviews = _review.GetAll()
                    .Where(r => r.WorkerId == worker.Id)
                    .Where(r => r.AuthorUserId != userGuid)
                    .OrderByDescending(r => r.CreatedDate)
                    .ToList();
            }
            else if (role == "Cliente")
            {
                var client = _client.GetAll()
                    .FirstOrDefault(c => c.UserId == userGuid);

                if (client == null)
                    return NotFound("No se encontró el perfil del cliente.");

                reviews = _review.GetAll()
                    .Where(r => r.ClientId == client.Id)
                    .Where(r => r.AuthorUserId != userGuid)
                    .OrderByDescending(r => r.CreatedDate)
                    .ToList();
            }
            else
            {
                return Forbid();
            }

            return Ok(reviews.Select(MapearRespuesta).ToList());
        }

        private ReviewResponseDto MapearRespuesta(Review review)
        {
            var resultado = _mapper.Map<ReviewResponseDto>(review);

            var client = _client.GetById(review.ClientId);
            var worker = _worker.GetById(review.WorkerId);

            var esCliente = client?.UserId == review.AuthorUserId;

            resultado.AuthorName = esCliente
                ? $"{client?.Name} {client?.LastName}"
                : $"{worker?.Name} {worker?.LastName}";

            resultado.RecipientName = esCliente
                ? $"{worker?.Name} {worker?.LastName}"
                : $"{client?.Name} {client?.LastName}";

            resultado.AuthorRole = esCliente ? "Cliente" : "Trabajador";

            return resultado;
        }
    }
}
