using AdLumeDash.Models;

namespace AdLumeDash.Interface;

public interface IEquipamentoRepository
{

    Task<IEnumerable<EquipamentoPlaylistDto>?> GetByGuid(string guid);
    //Task<Equipamento?> GetByGuid(string guid);
}
