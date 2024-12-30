using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieGPT.ApplicationServices;
using Xunit;

public class OpenAiControllerTests
{
    private readonly Mock<OpenAiService> _mockOpenAiService;
    private readonly OpenAiController _controller;

    public OpenAiControllerTests()
    {
        _mockOpenAiService = new Mock<OpenAiService>();
        _controller = new OpenAiController(_mockOpenAiService.Object);
    }

    [Fact]
    public async Task GetResult_Should_Return_View_With_Movie()
    {
        // Arrange
        var prompt = "Find a movie";
        var fakeMovie = new Movie { Title = "Inception", Genre = "Sci-Fi" };
        var fakeResult = JsonSerializer.Serialize(fakeMovie);

        _mockOpenAiService.Setup(service => service.GetResult(prompt))
            .ReturnsAsync(fakeResult);

        // Act
        var result = await _controller.GetResult(prompt);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var movies = Assert.IsType<List<Movie>>(viewResult.ViewData["Movies"]);
        Assert.Single(movies);
        Assert.Equal("Inception", movies[0].Title);
    }

    [Fact]
    public async Task GetResult_Should_Return_View_With_Empty_Movie_When_Null()
    {
        // Arrange
        var prompt = "Find a movie";
        _mockOpenAiService.Setup(service => service.GetResult(prompt))
            .ReturnsAsync("null");

        // Act
        var result = await _controller.GetResult(prompt);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<Movie>(viewResult.ViewData["Movie"]);
        Assert.Null(viewResult.ViewData["Movies"]);
    }

    [Fact]
    public async Task GetMoreThanOneResult_Should_Return_View_With_Movies()
    {
        // Arrange
        var prompt = "Find multiple movies";
        var fakeMovies = new List<Movie>
        {
            new Movie { Title = "Inception", Genre = "Sci-Fi" },
            new Movie { Title = "The Dark Knight", Genre = "Action" }
        };
        var fakeResult = JsonSerializer.Serialize(fakeMovies);

        _mockOpenAiService.Setup(service => service.GetMoreThanOneResult(prompt))
            .ReturnsAsync(fakeResult);

        // Act
        var result = await _controller.GetMoreThanOneResult(prompt);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var movies = Assert.IsType<List<Movie>>(viewResult.ViewData["Movies"]);
        Assert.Equal(2, movies.Count);
        Assert.Equal("Inception", movies[0].Title);
    }
}
