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