using Microsoft.AspNetCore.Mvc;

namespace AdLumeDash.Controllers
{
    //[ApiController]
    //public class ConfigController : ControllerBase
    //{
    //
    //    [HttpGet]
    //    [Route("device/{deviceId}/config")]
    //    public IActionResult GetConfig(Guid deviceId)
    //    {
    //        // 🔹 Mock inicial (depois vem do banco)
    //        var version = 1;
    //
    //        var media = new[]
    //        {
    //            new
    //            {
    //                hash = "a",
    //                url = $"{Request.Scheme}://{Request.Host}/media/a1.mp4",
    //                type = "video"
    //            }
    //        };
    //
    //        var playlists = new[]
    //        {
    //        new
    //        {
    //            id = "playlist-1",
    //            items = new[]
    //            {
    //                new
    //                {
    //                    mediaHash = "a",
    //                    duration = 0 // 0 = usa duração do vídeo
    //                }
    //            }
    //        }
    //    };
    //
    //        var schedule = new[]
    //        {
    //        new
    //        {
    //            playlistId = "playlist-1",
    //            start = "00:00",
    //            end = "23:59",
    //            days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" }
    //        }
    //    };
    //
    //        var config = new
    //        {
    //            version,
    //            media,
    //            playlists,
    //            schedule
    //        };
    //
    //        return Ok(config);
    //
    //    }
    //
    //}
}
