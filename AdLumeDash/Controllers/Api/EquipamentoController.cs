using AdLumeDash.Interface;
using AdLumeDash.Models.Api;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdLumeDash.Controllers.API
{
    [ApiController]
    public class EquipamentoController : ControllerBase
    {

        private readonly IEquipamentoRepository _repo;

        public EquipamentoController(IEquipamentoRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        [Route("Equipamento/{deviceId}")]
        public async Task<IActionResult> GetEquipamento(string deviceId)
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

            IEnumerable<EquipamentoPlaylistDto>? ret = await _repo.GetByGuid(deviceId);

            if (ret == null)
                return NotFound();

            return Ok(ret);


        }

    }
}
