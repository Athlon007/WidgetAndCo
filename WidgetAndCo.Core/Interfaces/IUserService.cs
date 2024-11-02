using WidgetAndCo.Core.DTOs;

namespace WidgetAndCo.Core.Interfaces;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByEmailAsync(string email);
    Task<UserResponseDto?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);

    /**
     * <summary>
     * Deletes a user with the given id.
     * </summary>
     * <param name="id">The id of the user to delete.</param>
     */
    Task DeleteUserAsync(Guid id);

    /**
     * <summary>
     * Registers a new user with the given information.
     * </summary>
     * <param name="registerUserDto">The information of the user to register.</param>
     */
    Task<UserResponseDto> RegisterUserAsync(RegisterUserDto registerUserDto);

    /**
     * <summary>
     * Logs in a user with the given email and password.
     * </summary>
     * <param name="loginUserDto">The email and password of the user to log in.</param>
     */
    Task<LoginResponseDto?> LoginUserAsync(LoginUserDto loginUserDto);
}