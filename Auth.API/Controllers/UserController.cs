using Auth.Application.UseCases.Users.Create;
using Auth.Application.UseCases.Users.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ICreateUser create) : BaseController<UserController>
    {
        private readonly ICreateUser _create = create;

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserOutput>> Create([FromForm] UserInput input)
        {
            UserOutput output = await _create.Execute(input);
            return output;
        }
    }
}