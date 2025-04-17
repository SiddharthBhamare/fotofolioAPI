using Dapper;
using fotofolioAPI.Entities;
using fotofolioAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Net;

namespace fotofolioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        IConfiguration _config;
        public ProfileController(IConfiguration config)
        {
            _config = config;
        }
        [HttpGet("getProfile")]
        public async Task<IActionResult> GetAll()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));

            var sql = @"SELECT id, profilepicture, email, bio, contactno FROM public.profile order by id desc";

            var result = await connection.QueryAsync(sql);

            var response = result.Select(r => new
            {
                Profilepicture = r.profilepicture != null ? Convert.ToBase64String((byte[])r.profilepicture) : null,
                Email = r.email,
                Bio = r.bio,
                ContactNo = r.contactno
            });

            return Ok(response);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> Insert([FromForm] Profile? profile)
        {
            byte[] imageData = null;

            if (profile.ProfilePicture != null)
            {
                using var ms = new MemoryStream();
                await profile.ProfilePicture.CopyToAsync(ms);
                imageData = ms.ToArray();
            }



            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = @"INSERT INTO public.profile (profilepicture, email, bio, contactno)
            VALUES (@ProfilePicture, @Email, @Bio, @ContactNo)";

            var Data = new 
            {
                ProfilePicture = imageData,
                Bio = profile.Bio, 
                ContactNo = profile.ContactNo,
                Email = profile.Email
            };
            await connection.ExecuteAsync(sql, Data);

            return Ok("Profile Created successfully.");
        }
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok("Connected : "+_config.GetConnectionString("DefaultConnection"));
        }
    }
}
