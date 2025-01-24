using Microsoft.Extensions.Logging;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Business
{
    public class UserBusiness(ILogger<UserBusiness> logger,
        IUserRepository userRepository) : IUserBusiness
    {
        private readonly ILogger<UserBusiness> _logger = logger;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<ActionResult<User>> CheckUserExists(int userId)
        {
            try
            {
                if (!await _userRepository.Exists(userId))
                    return new ActionResult<User>
                    {
                        Success = false,
                        Message = $"User with ID {userId} does not found."
                    };

                return new ActionResult<User>
                {
                    Success = true,
                    Message = $"User with ID {userId} was found."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while checking existence of user with ID {userId}. Exception: {ex.Message}");
                return new ActionResult<User>
                {
                    Success = false,
                    Message = "An error occurred while processing the request."
                };
            }
        }

        public async Task<ActionResult<User>> CheckUserRole(int userId)
        {
            try
            {
                var user = await _userRepository.Find(userId);

                if (user is null)
                    return new ActionResult<User>
                    {
                        Success = false,
                        Message = $"User with ID {userId} does not found."
                    };

                if (!user.Role.Equals("Gerente", StringComparison.OrdinalIgnoreCase))
                    return new ActionResult<User>
                    {
                        Success = false,
                        Message = $"You do not have the required permissions to perform this action. Only users with the 'Gerente' role are authorized."
                    };

                return new ActionResult<User>
                {
                    Success = true,
                    Message = $"User with ID {userId} was found."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while checking existence of user with ID {userId}. Exception: {ex.Message}");
                return new ActionResult<User>
                {
                    Success = false,
                    Message = "An error occurred while processing the request."
                };
            }
        }

        public async Task<ActionResult<User>> CreateUser(UserModel userModel)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userModel.Username))
                {
                    return new ActionResult<User>
                    {
                        Success = false,
                        Message = "Username cannot be empty. Please provide a valid username."
                    };
                }

                if (string.IsNullOrWhiteSpace(userModel.Role))
                {
                    return new ActionResult<User>
                    {
                        Success = false,
                        Message = "Role cannot be empty. Please provide a valid role."
                    };
                }

                var user = new User()
                {
                    UserName = userModel.Username,
                    Role = userModel.Role
                };

                user = await _userRepository.Insert(user);

                return new ActionResult<User>
                {
                    Success = true,
                    Message = "User created successfully.",
                    Result = user
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while trying to create the user: {ex.Message}", ex);
                return new ActionResult<User>
                {
                    Success = false,
                    Message = "An error occurred while creating the user. Please try again later."
                };
            }
        }
    }
}
