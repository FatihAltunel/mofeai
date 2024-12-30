using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication;

public class AccountControllerTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        var store = new Mock<IUserStore<User>>(); 
        _mockUserManager = new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);

        // Mock IHttpContextAccessor, IUserClaimsPrincipalFactory, IdentityOptions, ILogger, IAuthenticationSchemeProvider
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var mockUserClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        var mockOptions = new Mock<IOptions<IdentityOptions>>();
        var mockLogger = new Mock<ILogger<SignInManager<User>>>();
        var mockSchemeProvider = new Mock<IAuthenticationSchemeProvider>();

        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            mockHttpContextAccessor.Object,
            mockUserClaimsFactory.Object,
            mockOptions.Object,
            mockLogger.Object,
            mockSchemeProvider.Object,
            null);

        _controller = new AccountController(_mockSignInManager.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsRedirectToHome()
    {
    // Arrange
        var user = new User { UserName = "testuser", Email = "test@example.com" };
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _controller.Login(new LoginViewModel { Email = "testuser", Password = "password" });

        // Assert
        var redirectResult = Assert.IsType<Microsoft.AspNetCore.Mvc.RedirectToActionResult>(result);
        Assert.Equal("Home", redirectResult.ControllerName);
    }   


    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsErrorMessage()
    {
        // Arrange
        _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        // Act
        var result = await _controller.Login(new LoginViewModel { Email = "invaliduser", Password = "wrongpassword" });

        // Assert
        var viewResult = Assert.IsType<Microsoft.AspNetCore.Mvc.ViewResult>(result);
        var errorMessage = viewResult.ViewData["ErrorMessage"] as string;
        Assert.Equal("Invalid credentials", errorMessage);  // Burada errorMessage'ı kontrol ediyoruz
    }

}
