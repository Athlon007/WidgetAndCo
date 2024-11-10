using AutoMapper;
using Microsoft.Extensions.Configuration;
using WidgetAndCo.Business;
using WidgetAndCo.Core;
using WidgetAndCo.Core.DTOs;
using WidgetAndCo.Core.Interfaces;

namespace WidgetAndCo.Tests;

[TestFixture]
public class UserServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IMapper> _mapperMock;
    private Mock<IConfiguration> _configurationMock;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _configurationMock = new Mock<IConfiguration>();
        _userService = new UserService(_userRepositoryMock.Object, _configurationMock.Object, _mapperMock.Object);

        var jwtSettingsSection = new Mock<IConfigurationSection>();
        _configurationMock.Setup(config => config.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);
        jwtSettingsSection.Setup(c => c["SecretKey"]).Returns("thisisatestingsupersecretkey123456789");
        jwtSettingsSection.Setup(c => c["AccessTokenExpirationHours"]).Returns("1");
        jwtSettingsSection.Setup(c => c["Issuer"]).Returns("issuer");
        jwtSettingsSection.Setup(c => c["Audience"]).Returns("audience");
    }

    private User GetUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123"),
            Role = RoleEnum.User,
            CreatedAt = DateTime.UtcNow,
            LastLogin = DateTime.UtcNow
        };
    }

    private UserResponseDto GetUserResponseDto()
    {
        return new UserResponseDto(
            Guid.NewGuid(),
            "John",
            "Doe",
            "text@example.com",
            "User",
            DateTime.UtcNow,
            DateTime.UtcNow
        );
    }

    private RegisterUserDto GetRegisterUserDto()
    {
        return new RegisterUserDto("test@example.com", "John", "Doe", "Password123");
    }

    private LoginUserDto GetLoginUserDto()
    {
        return new LoginUserDto("text@example.com", "Password123");
    }

    // Test for GetUserByEmailAsync
    [Test]
    public async Task GetUserByEmailAsync_ShouldReturnMappedUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var user = GetUser();
        var userDto = GetUserResponseDto();


        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result?.Email, Is.EqualTo(userDto.Email));
    }

    [Test]
    public async Task GetUserByEmailAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var email = "nonexistent@example.com";
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(email)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByEmailAsync(email);

        // Assert
        Assert.IsNull(result);
    }

    // Test for GetUserByIdAsync
    [Test]
    public async Task GetUserByIdAsync_ShouldReturnMappedUser_WhenUserExists()
    {
        // Arrange
        var user = GetUser();
        var userDto = GetUserResponseDto();
        var id = user.Id;


        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(id)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserResponseDto>(user)).Returns(userDto);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result?.Id, Is.EqualTo(userDto.Id));
    }

    [Test]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(id)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(id);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task RegisterUserAsync_ShouldAddUserAndReturnMappedUser()
    {
        // Arrange
        var registerUserDto = new RegisterUserDto
        {
            Email = "newuser@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "Password123"
        };

        var userDto = new UserResponseDto { Email = registerUserDto.Email };

        _mapperMock
            .Setup(m => m.Map<UserResponseDto>(It.IsAny<User>()))
            .Returns(userDto);

        _userRepositoryMock
            .Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterUserAsync(registerUserDto);

        // Assert
        _userRepositoryMock.Verify(repo => repo.AddUserAsync(It.IsAny<User>()), Times.Once);
        Assert.IsNotNull(result);
        Assert.AreEqual(userDto.Email, result.Email);
        Assert.AreEqual(userDto.FirstName, result.FirstName);

    }

    // Test for LoginUserAsync
    [Test]
    public async Task LoginUserAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var loginUserDto = GetLoginUserDto();
        var user = new User
        {
            Email = loginUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginUserDto.Password)
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginUserDto.Email)).ReturnsAsync(user);

        // Act
        var result = await _userService.LoginUserAsync(loginUserDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result.Token);
    }

    [Test]
    public async Task LoginUserAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var loginUserDto = new LoginUserDto("test@example.com", "InvalidPassword");
        var user = GetUser();
        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginUserDto.Email)).ReturnsAsync(user);

        // Act
        var result = await _userService.LoginUserAsync(loginUserDto);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task ValidateToken_ShouldReturnTrue_WhenTokenIsValid()
    {
        // Arrange
        var loginUserDto = GetLoginUserDto();
        var user = new User
        {
            Email = loginUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginUserDto.Password)
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginUserDto.Email)).ReturnsAsync(user);

        // Act
        // Generate token
        var loginResponse = await _userService.LoginUserAsync(loginUserDto);
        // Validate token
        var result = _userService.ValidateToken(loginResponse.Token);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task ValidateToken_ShouldReturnFalse_WhenTokenIsInvalid()
    {
        // Arrange
        var token = "invalidtoken";

        // Act
        var result = _userService.ValidateToken(token);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task GetUserIdFromToken_ShouldReturnUserId_WhenTokenIsValid()
    {
        // Arrange
        var loginUserDto = GetLoginUserDto();
        var user = new User
        {
            Email = loginUserDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(loginUserDto.Password)
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(loginUserDto.Email)).ReturnsAsync(user);

        // Act
        // Generate token
        var loginResponse = await _userService.LoginUserAsync(loginUserDto);
        // Get user ID from token
        var result = _userService.GetUserIdFromToken(loginResponse.Token);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOf<Guid>(result);
    }
}
