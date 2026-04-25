using Microsoft.AspNetCore.Mvc;

namespace AdLumeDash.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var path = Path.Combine("storage", file.FileName);

            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);

            return Ok();
        }

        [HttpGet("{fileName}")]
        public IActionResult GetMedia(string fileName)
        {

            var authHeader = Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader))
            {
                return Unauthorized();
            }

            // Exemplo: validação simples (string fixa)
            if (!authHeader.Contains("bbsinfo.com.br"))
            {
                return Unauthorized();
            }

            var path = Path.Combine("storage", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var stream = System.IO.File.OpenRead(path);
            return File(stream, "application/octet-stream");
        }

    }
}
