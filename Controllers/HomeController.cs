using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieGPT.Models;
using MovieGPT.ApplicationServices;

namespace MovieGPT.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly OpenAiService _openAiService;

    public HomeController(ILogger<HomeController> logger, OpenAiService openAiService)
    {
        _logger = logger;
        _openAiService = openAiService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
