using Auth.API.Filters;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController<AuthController>
    {
        public AuthController()
        {

        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult<DateTime> Get()
        {
            return Ok(GerarHorarioBrasilia());
        }

        [AuthorizeFilter(UserRoleEnum.Administrador)]
        [HttpGet("Authorized")]
        public ActionResult<DateTime> GetAuthorized()
        {
            return GerarHorarioBrasilia();
        }

        [AllowAnonymous]
        [HttpPost("Auth")]
        public ActionResult<Guid> Auth()
        {
            if (IsAuth())
            {
                throw new Exception("Este usuário já está autenticado");
            }

            return Guid.NewGuid();
        }
    }
}