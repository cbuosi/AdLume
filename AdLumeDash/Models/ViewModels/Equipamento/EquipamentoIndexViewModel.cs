namespace AdLumeDash.Models.ViewModels.Equipamento;

public class EquipamentoIndexViewModel
{
    public List<EquipamentoModel> Itens { get; set; } = new();
}

public class EquipamentoModel
{
    public int cEquipamento { get; set; }
    public string NomeEquipamento { get; set; } = string.Empty;
    public string? DescEquipamento { get; set; }
}
