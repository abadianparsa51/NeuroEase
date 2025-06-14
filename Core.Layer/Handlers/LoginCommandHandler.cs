// Core.Layer/Handlers/LoginCommandHandler.cs
using Application.Layer.Authontication.Command;
using Application.Layer.Interface;
using Core.Layer.Helpers;
using Core.Model.Layer.Entity;
using Core.Model.Layer.Model;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Core.Layer.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResult>
    {
        private readonly IAuthenticationRepository _authRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IAuthenticationRepository authRepository, SignInManager<ApplicationUser> signInManager, IJwtHelper jwtHelper, ILogger<LoginCommandHandler> logger, UserManager<ApplicationUser> userManager)
        {
            _authRepository = authRepository;
            _signInManager = signInManager;
            _jwtHelper = jwtHelper;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Processing login for email: {request.Email}");

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning($"User not found: {request.Email}");
                return new AuthResult
                {
                    Success = false,
                    Message = "کاربر یافت نشد"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

            if (result.Succeeded)
            {
                var token = _jwtHelper.GenerateJwtToken(user);
                _logger.LogInformation($"User logged in successfully: {request.Email}");
                return new AuthResult
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Result = true,
                    Success = true,
                    Message = "ورود موفقیت‌آمیز بود",
                    Token = token
                };
            }

            _logger.LogError($"Login failed for {request.Email}: Incorrect password");
            return new AuthResult
            {
                Success = false,
                Message = "رمز عبور اشتباه است"
            };
        }
    }
}