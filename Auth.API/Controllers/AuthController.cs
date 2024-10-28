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
        public DateTime Get()
        {
            return GerarHorarioBrasilia();
        }

        [AuthorizeFilter(UserRoleEnum.Administrador)]
        [HttpGet("Authorized")]
        public DateTime GetAuthorized()
        {
            return GerarHorarioBrasilia();
        }
    }
}