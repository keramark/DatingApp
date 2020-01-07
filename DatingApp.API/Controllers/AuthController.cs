using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
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
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) //now user and password place holders in future json
        {

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if(await _repo.UserExists( userForRegisterDto.Username))
                return BadRequest("Username already exisits");


            var userToCreate = new User //createing user 
            {
                Username =  userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); //we cheated it should be CreatedAtRoute() but we don't have user yet


        }



        
    }
}