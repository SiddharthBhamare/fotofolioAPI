using Dapper;
using fotofolioAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

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

            var sql = @"SELECT id, profilepicture, email, bio, contactno FROM public.profile order by id desc limit 1";

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
        public async Task<IActionResult> Insert(Profile? profile)
        {
            if (profile == null)
                return BadRequest("Profile is null.");

            byte[] imageData = null;

            if (profile.ProfilePicture != null)
            {
                using var ms = new MemoryStream();
                await profile.ProfilePicture.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = @"
        DELETE FROM public.profile;

        INSERT INTO public.profile (profilepicture, email, bio, contactno)
        VALUES (@ProfilePicture, @Email, @Bio, @ContactNo);";

            var data = new
            {
                ProfilePicture = imageData,
                Bio = profile.Bio,
                ContactNo = profile.ContactNo,
                Email = profile.Email
            };

            await connection.ExecuteAsync(sql, data);

            return Ok("Profile created successfully.");
        }

    }
}
