using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MovieGPT.ApplicationServices;
public class OpenAiController : Controller
{
    private readonly OpenAiService _openAiService;

    public OpenAiController(OpenAiService openAiService)
    {
        _openAiService = openAiService;
    }

    [HttpPost]
    public async Task<IActionResult> GetResult(string prompt){

    var result = await _openAiService.GetResult(prompt);

    var movie = JsonSerializer.Deserialize<Movie>(result);
    var movies = new List<Movie>(); 
    if(movie != null){
        movies.Add(movie);
    }
    else{
        ViewBag.Movie = new Movie();
    }

    ViewBag.prompt = prompt;
    ViewBag.Movies = movies;

    return View("/Views/Home/Index.cshtml");
}

[HttpPost]
    public async Task<IActionResult> GetMoreThanOneResult(string prompt){

        var result = await _openAiService.GetMoreThanOneResult(prompt);

        var movies = JsonSerializer.Deserialize<List<Movie>>(result);

        ViewBag.prompt = prompt;
        ViewBag.Movies = movies;
    
    return View("/Views/Home/Index.cshtml");
}
}