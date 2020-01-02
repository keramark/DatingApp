using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }


        [HttpPost ("register")]
        public async Task<IActionResult> Register(string username, string password) //now user and password place holders in future json
        {
            //validate request not yet

            username = username.ToLower();

            if(await _repo.UserExists(username))
                return BadRequest("Username already exisits");


            var userToCreate = new User //createing user 
            {
                Username = username
            };

            var createdUser = await _repo.Register(userToCreate, password);

            return StatusCode(201); //we cheated it should be CreatedAtRoute() but we don't have user yet


        }



        
    }
}