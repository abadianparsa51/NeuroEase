using Core.Model.Layer.Entity;
using Microsoft.AspNetCore.Identity;
using Core.Layer.Helpers;
using Microsoft.Extensions.Logging;
using Application.Layer.Authontication.Dto;
using Application.Layer.Interface;
using Core.Model.Layer.Model;

namespace Core.Layer.Repository
{
    public class AuthRepository : IAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtHelper _jwtHelper;
        public ILogger<AuthRepository> Register { get; }

        public AuthRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IJwtHelper jwtHelper, ILogger<AuthRepository> repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtHelper = jwtHelper;
            Register = repository;
        }

        public async Task<AuthResult> RegisterAsync(UserLoginRequestDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                return new AuthResult
                {
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Result = true,
                    Success = true,
                    Message = "ثبت‌نام با موفقیت انجام شد"
                };
            }

            return new AuthResult
            {
                Success = false,
                Message = string.Join("; ", result.Errors.Select(e => e.Description))
            };
        }

        public async Task<AuthResult> LoginAsync(UserLoginRequestDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "کاربر یافت نشد"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (result.Succeeded)
            {
                var token = _jwtHelper.GenerateJwtToken(user); // تولید توکن با JwtHelper
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

            return new AuthResult
            {
                Success = false,
                Message = "رمز عبور اشتباه است"
            };
        }
    }
}