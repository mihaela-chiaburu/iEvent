using iEvent.Application.DTOs.User;
using iEvent.Application.Exceptions;
using iEvent.Application.Interfaces.Repositories;
using iEvent.Application.Interfaces.Services;
using iEvent.Domain.Enums;

namespace iEvent.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileService _userProfileService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IUserProfileService userProfileService,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _userProfileService = userProfileService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var existing = await _userRepository.ExistsByEmailAsync(request.Email);
            if (existing)
            {
                throw new ConflictException("User already exists.");
            }

            var createResult = await _userRepository.CreateUserWithRoleAsync(
                request.Email,
                request.Password,
                RoleNames.Customer,
                request.PhoneNumber);

            if (!createResult.Succeeded)
            {
                throw new ValidationException(
                    "User registration failed.",
                    createResult.Errors ?? Array.Empty<string>());
            }

            if (string.IsNullOrWhiteSpace(createResult.CreatedUserId))
            {
                throw new InvalidOperationException("User registration succeeded but no user ID was returned.");
            }

            await _userProfileService.CreateCustomerProfileAsync(
                createResult.CreatedUserId,
                request.Email,
                request.UserName,
                request.PhoneNumber);
        }

        public async Task<AuthLoginRespDto> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.ValidateCredentialsAsync(request.Email, request.Password);
            if (user == null)
            {
                throw new UnauthorizedException("Invalid email or password.");
            }

            var token = _jwtTokenService.GenerateToken(user.Value.Id, user.Value.Email, user.Value.Roles);

            return new AuthLoginRespDto
            {
                Token = token,
                Roles = user.Value.Roles
            };
        }
    }
}
