using Dapper;
using fotofolioAPI.Entities;
using fotofolioAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace fotofolioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RawDataController : ControllerBase
    {
        IConfiguration _config;
        public RawDataController(IConfiguration config)
        {
            _config = config;
        }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(UploadRequest? UploadRequest)
        {
            byte[] imageData = null;

            if (UploadRequest.Image != null)
            {
                using var ms = new MemoryStream();
                await UploadRequest.Image.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

           

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));
            var sql = @"INSERT INTO public.Rawdata (title, category, image, youtubeurl)
            VALUES (@Title, @Category, @Image, @YoutubeURL)";

            var Data = new RawData
            {
                Title = UploadRequest.Title,
                Category = UploadRequest.Category, // Replace with actual subcategory if needed
                Image = imageData,
                YoutubeURL = UploadRequest.YouTubeLink
            };
            await connection.ExecuteAsync(sql, Data);

            return Ok("Category uploaded successfully.");
        }
        [HttpGet("ShowMetdaData")]
        public async Task<IActionResult> ShowMetdaData()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));

            var sql = @"SELECT id, title, category, youtubeurl FROM public.Rawdata";

            var result = await connection.QueryAsync(sql);

            var response = result.Select(r => new
            {
                Id = r.id,
                Title = r.title,
                Category = r.category,
                YoutubeURL = r.youtubeurl
            });

            return Ok(response);
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));

            var sql = @"SELECT id, title, category, image, youtubeurl FROM public.Rawdata";

            var result = await connection.QueryAsync(sql);

            var response = result.Select(r => new 
            {
                Id = r.id,
                Title = r.title,
                Category = r.category,
                YoutubeURL = r.youtubeurl,
                Image = r.image != null ? Convert.ToBase64String((byte[])r.image) : null
            });

            return Ok(response);
        }

        [HttpGet("getAllIds")]
        public async Task<IActionResult> GetAllIds()
        {
            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));

            var sql = @"SELECT id FROM public.Rawdata";

            var result = await connection.QueryAsync(sql);

            var response = result.Select(r => new
            {
                Id = r.id
            });

            return Ok(response);
        }
        [HttpDelete("DeleteByIds")]
        public async Task<IActionResult> Delete([FromBody] List<Guid> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("No IDs provided for deletion.");

            using var connection = new NpgsqlConnection(_config.GetConnectionString("DefaultConnection"));

            var sql = @"DELETE FROM public.Rawdata WHERE id = ANY(@Ids)";

            var affectedRows = await connection.ExecuteAsync(sql, new { Ids = ids });

            return Ok(new { Message = "Deleted successfully", DeletedCount = affectedRows });
        }


    }
}
