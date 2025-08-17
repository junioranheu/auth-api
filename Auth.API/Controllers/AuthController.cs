using Auth.Application.UseCases.Auth.CreateTokenJWT;
using Auth.Application.UseCases.Auth.Shared;
using Auth.Application.UseCases.Users.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(ICreateToken createToken) : BaseController<AuthController>
    {
        private readonly ICreateToken _createToken = createToken;

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserOutput>> Auth(AuthInput input)
        {
            if (IsAuth())
            {
                throw new Exception("Este usuário já está autenticado");
            }

            UserOutput output = await _createToken.Execute(input);
            return Ok(output);
        }
    }
}