using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Oficios.Abstractions;
using Oficios.Applications.Dtos.Identity.User;
using Oficios.Applications.Dtos.Login;
using Oficios.Entities.MicrosoftEntity;
using Oficios.Services.AuthServices;
using Oficios.WebApi.Configurations;

namespace Oficios.WebApi.Controllers.Identity
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<WorkersController> _logger;
        private readonly ITokenHandlerService _servicioToken;
        public AuthController(
            UserManager<User> userManager
            , ILogger<WorkersController> logger
            , ITokenHandlerService servicioToken)
        {
            _userManager = userManager;
            _logger = logger;
            _servicioToken = servicioToken;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UserRegistroRequestDto user)
        {
            if (ModelState.IsValid)
            {
                var existeUsuario = await _userManager.FindByEmailAsync(user.Email);
                if (existeUsuario != null)
                {
                    return BadRequest("Existe un usuario registrado con el mal " + user.Email + ".");
                }
                var Creado = await _userManager.CreateAsync(new User()
                {
                    Email = user.Email,
                    UserName = user.Email.Substring(0, user.Email.IndexOf('@')),
                    Nombres = user.Nombres,
                    Apellidos = user.Apellidos,
                    FechaNacimiento = user.FechaNacimiento
                }, user.Password);
                if (Creado.Succeeded)
                {
                    return Ok(new UserRegistroResponseDto
                    {
                        NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                        Email = user.Email,
                        UserName = user.Email.Substring(0, user.Email.IndexOf('@'))
                    });
                }
                else
                {
                    return BadRequest(Creado.Errors.Select(e => e.Description).ToList());
                }
            }
            else
            {
                return BadRequest("Los datos enviados no son validos.");
            }
        }

        [HttpPost]
        [Route("RegisterSync")]
        public IActionResult RegistrarUsuarioSincronico([FromBody] UserRegistroRequestDto user)
        {
            if (ModelState.IsValid)
            {
                var existeUsuario = _userManager.FindByEmailAsync(user.Email).GetAwaiter().GetResult();
                if (existeUsuario != null)
                {
                    return BadRequest("Existe un usuario registrado con el mail " + user.Email + ".");
                }
                var username = user.Email.Substring(0, user.Email.IndexOf('@'));
                var creado = _userManager.CreateAsync(new User()
                {
                    Email = user.Email,
                    UserName = username,
                    Nombres = user.Nombres,
                    Apellidos = user.Apellidos,
                    FechaNacimiento = user.FechaNacimiento
                }, user.Password).GetAwaiter().GetResult();

                if (creado.Succeeded)
                {
                    return Ok(new UserRegistroResponseDto
                    {
                        NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                        Email = user.Email,
                        UserName = username
                    });
                }
                else
                {
                    return BadRequest(creado.Errors.Select(e => e.Description).ToList());
                }
            }
            else
            {
                return BadRequest("Los datos enviados no son validos.");
            }
        }

        [HttpPost]
        [Route("RegisterWithToken")]
        public async Task<IActionResult> RegistrarUsuarioConToken([FromBody] UserRegistroRequestDto user, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var existeUsuario = await _userManager.FindByEmailAsync(user.Email);
                if (existeUsuario != null)
                {
                    return BadRequest("Existe un usuario registrado con el mail " + user.Email + ".");
                }
                cancellationToken.ThrowIfCancellationRequested();
                var username = user.Email.Substring(0, user.Email.IndexOf('@'));
                var creado = await _userManager.CreateAsync(new User()
                {
                    Email = user.Email,
                    UserName = username,
                    Nombres = user.Nombres,
                    Apellidos = user.Apellidos,
                    FechaNacimiento = user.FechaNacimiento
                }, user.Password);
                if (creado.Succeeded)
                {
                    return Ok(new UserRegistroResponseDto
                    {
                        NombreCompleto = string.Join(" ", user.Nombres, user.Apellidos),
                        Email = user.Email,
                        UserName = username
                    });
                }
                else
                {
                    return BadRequest(creado.Errors.Select(e => e.Description).ToList());
                }
            }
            else
            {
                return BadRequest("Los datos enviados no son validos.");
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserRequestDto userlogin)
        {
            if (ModelState.IsValid)
            {
                var existeUsuario = await _userManager.FindByEmailAsync(userlogin.Email);
                if (existeUsuario != null)
                {
                    var isCorrect = await _userManager.CheckPasswordAsync(existeUsuario, userlogin.Password);
                    if (isCorrect)
                    {
                        try
                        {
                            var roles = await _userManager.GetRolesAsync(existeUsuario);
                            var parametros = new TokenParameters()
                            {
                                Id = existeUsuario.Id.ToString(),
                                PaswordHash = existeUsuario.PasswordHash,
                                UserName = existeUsuario.UserName,
                                Email = existeUsuario.Email,
                                Roles = roles
                            };
                            var jwt = _servicioToken.GenerateJwtTokens(parametros);
                            return Ok(new LoginUserResponseDto()
                            {
                                Login = true,
                                Token = jwt,
                                UserName = existeUsuario.UserName,
                                Mail = existeUsuario.Email,
                                Role = roles.FirstOrDefault()
                            });
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }
            return BadRequest(new LoginUserResponseDto()
            {
                Login = false,
                Errores = new List<string>()
                    {
                       "Usuario o contraseña incorrecto!"
                    }
            });
        }
    }
}
