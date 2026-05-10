using Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Dto;
using Repository.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace project1.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class Userscontroller : ControllerBase
    {
        private const string AuthCookieName = "auth_token";
        IUserServices _s;
        private readonly ILogger<Userscontroller> _logger;
        public Userscontroller(IUserServices i, ILogger<Userscontroller> logger)
        {
            _logger = logger;
            _s = i;
        }

        //GET: api/<users>
        [AuthorizeRole("Admin")]
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            return await _s.GetUsers();
        }

        // GET api/<users>/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoUser_Name_Gmail_Role_Id>> Get(int id)
        {
            DtoUser_Name_Gmail_Role_Id user = await _s.GetUserById(id);
            if (user!=null)
            {
                return Ok(user);
            }       
          return NoContent();
        }
        // POST api/<users>

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<DtoAuthResponse>> Post([FromBody] DtoUser_All user)
        {

            DtoAuthResponse res = await _s.AddNewUser(user);
            if (res!=null)
            {
                WriteAuthCookie(res.Token);
                return CreatedAtAction(nameof(Get), new { id = res.User.UserId }, res);
            }
            else
                return BadRequest();
        }

        //POST
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<DtoAuthResponse>> Login([FromBody] DtoUser_Gmail_Password user)
        {
            DtoAuthResponse res = await _s.Login(user);
            if(res!=null)
            {
                WriteAuthCookie(res.Token);
                _logger.LogInformation("User login succeeded for email {Email}", user.Email);
                return Ok(res);
            }  
            return NotFound();
        }



        // PUT api/<users>/5
        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<DtoUser_Name_Gmail_Role_Id>> Put(int id, [FromBody] DtoUser_All value)
        {
            DtoUser_Name_Gmail_Role_Id res = await _s.update(id, value);
            if (res!= null)
            {
                return CreatedAtAction(nameof(Get), new { id = res.UserId }, res);
            }
            else
                return BadRequest();  
        }

        private void WriteAuthCookie(string token)
        {
            Response.Cookies.Append(AuthCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddHours(2)
            });
        }
    }
}
