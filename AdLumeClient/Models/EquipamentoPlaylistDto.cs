using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdLumeClient.Models;

public class EquipamentoPlaylistDto
{
    // --------------------------------------------------------------------
    // Equipamento
    public int cEquipamento { get; set; }
    public string? GuidEquipamento { get; set; }
    public string? NomeEquipamento { get; set; }
    public string? DescEquipamento { get; set; }
    public DateTime DtUltAtu { get; set; }

    // --------------------------------------------------------------------
    // Playlist
    public int cPlaylist { get; set; }
    public string? NomePlaylist { get; set; }
    public string? HoraInicio { get; set; }
    public string? HoraFim { get; set; }

    // --------------------------------------------------------------------
    // PlaylistItem
    public int cPlaylistItem { get; set; }
    public int Ordem { get; set; }
    public int cMidia { get; set; }

    // --------------------------------------------------------------------
    // Midia
    public string? DescMidia { get; set; }
    public string? NomeMidia { get; set; }
    public string? HashMidia { get; set; }
    public string? UrlMidia { get; set; }
    // --------------------------------------------------------------------

    public int MinIni()
    {
        return clsUtil.HoraParaInt(HoraInicio);
    }

    public int MinFin()
    {
        return clsUtil.HoraParaInt(HoraFim);
    }

}