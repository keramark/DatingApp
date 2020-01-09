using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) //now user and password place holders in future json
        {

            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("Username already exisits");


            var userToCreate = new User //createing user 
            {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201); //we cheated it should be CreatedAtRoute() but we don't have user yet
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            //check our repo for user
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            // if usernot exiist we will not give a hint to hacker we will only return unauthorize
            if (userFromRepo == null)
                return Unauthorized();

            //build up a token which will be return to the user
            //our token will be contain to bits of intormation about the user
            // user id and username


            //using System.Security.Claims;
            //here we store our claims from db 
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            //using Microsoft.IdentityModel.Tokens;
            //in order to make sure that our token is valid when comes back the server needs to sign him
            // we need to encode security key which is string to byte array
            // and we will store our key to AppSettings because we will use it in couple places
            // so we need to inject our configuration 
            // and create in appsettings
            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            //now we generate signing credentials

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            //now we need security signing descriptor which is going to contain our
            //claims our expire date and the signing credentials

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = System.DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            //we need token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            //and now with using our token handler we can create a token and pass in the token descriptor
            //so we'll save our token 
            //co it contain our tokenDescriptor so our JWT token which we wanna return to client !
            var token = tokenHandler.CreateToken(tokenDescriptor);


            //we send a token in the response to client
            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}