using Auth.API.Filters;
using Auth.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static junioranheu_utils_package.Fixtures.Get;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : BaseController<TestController>
    {
        [AllowAnonymous]
        [HttpGet("GetAnonymous")]
        public ActionResult<DateTime> GetAnonymous()
        {
            return Ok(GerarHorarioBrasilia());
        }

        [AuthorizeFilter(UserRoleEnum.Administrador, UserRoleEnum.Suporte)]
        [HttpGet("GetAuth")]
        public ActionResult<DateTime> GetAuth()
        {
            return GerarHorarioBrasilia();
        }
    }
}