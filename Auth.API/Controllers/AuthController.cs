using Auth.API.Filters;
using Auth.Application.UseCases.Auth.CreateTokenJWT;
using Auth.Application.UseCases.Auth.Shared;
using Auth.Application.UseCases.Users.Shared;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ICreateToken createToken) : BaseController<AuthController>
    {
        private readonly ICreateToken _createToken = createToken;

        [AllowAnonymous]
        [HttpGet("Get")]
        public ActionResult<DateTime> Get()
        {
            return Ok(GerarHorarioBrasilia());
        }

        [AuthorizeFilter(UserRoleEnum.Administrador)]
        [HttpGet("GetAuth")]
        public ActionResult<DateTime> GetAuth()
        {
            if (!IsAuth())
            {
                throw new Exception("É necessário estar autenticado para acessar esse end-point");
            }

            return GerarHorarioBrasilia();
        }

        [AllowAnonymous]
        [HttpPost("Auth")]
        public async Task<ActionResult<UserOutput>> Auth(AuthInput input)
        {
            if (IsAuth())
            {
                throw new Exception("Este usuário já está autenticado");
            }

            UserOutput output = await _createToken.Execute(input);
            return output;
        }
    }
}