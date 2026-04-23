using AdLumeDash.Interface;
using AdLumeDash.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace AdLumeDash.Controllers
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

            IEnumerable<EquipamentoPlaylistDto>? ret = await _repo.GetByGuid(deviceId);

            if (ret == null)
                return NotFound();

            return Ok(ret);


        }

    }
}
