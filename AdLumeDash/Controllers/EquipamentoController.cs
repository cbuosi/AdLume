using Microsoft.AspNetCore.Mvc;
using AdLumeDash.Models.ViewModels.Equipamento;

namespace AdLumeDash.Controllers;

public class EquipamentoController : Controller
{
    public IActionResult Index()
    {
        var vm = new EquipamentoIndexViewModel();

        // por enquanto vazio
        return View(vm);
    }
}