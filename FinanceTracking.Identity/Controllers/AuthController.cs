using AutoMapper;
using FinanceTracking.Identity.DTOs;
using FinanceTracking.Identity.Entities;
using FinanceTracking.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace FinanceTracking.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthController(IServiceProvider provider)
        {
            _userManager = provider.GetRequiredService<UserManager<AppUser>>();
            _signInManager = provider.GetRequiredService<SignInManager<AppUser>>();
            _mapper = provider.GetRequiredService<IMapper>();
            _tokenService = provider.GetRequiredService<ITokenService>();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                AppUser user = _mapper.Map<AppUser>(registerDto);

                IdentityResult createdUser = await _userManager.CreateAsync(user, registerDto.Password);
                if (!createdUser.Succeeded)
                    return StatusCode(500, createdUser.Errors);

                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                if (!roleResult.Succeeded)
                    return StatusCode(500, roleResult.Errors);

                return Ok(_mapper.Map<UserDto>(user));

            }
            catch (Exception exception)
            {
                return StatusCode(500, exception);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto loginDto, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(e => e.UserName == loginDto.UserName, ct);
            if (user == null)
                return Unauthorized("Invalid Username");

            SignInResult isPasswordValid = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!isPasswordValid.Succeeded)
                return Unauthorized("User not found with Username or Password");

            UserDto userDto = _mapper.Map<UserDto>(user);
            userDto.AccessToken = _tokenService.GenerateToken(user);
            return Ok(userDto);
        }
    }
}