using System.Diagnostics;
using _14548_employes_managment.Models;
using Microsoft.AspNetCore.Mvc;

namespace _14548_employes_managment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Pagina inicial da aplicacao.
        public IActionResult Index()
        {
            return View();
        }

        // Pagina simples de informacao/legal.
        public IActionResult Privacy()
        {
            return View();
        }

        // Mostra a pagina de erro sem guardar a cache da resposta.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
