using ShopZone.DTOs.Request.Auth;
using ShopZone.DTOs.Response.Auth;
using ShopZone.DTOs.Response.Common;
using ShopZone.Helpers;
using ShopZone.Models;
using ShopZone.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ShopZone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IOtpService _otpService;

        public AuthController(
            IGenericRepository<User> userRepository,
            IJwtService jwtService,
            IOtpService otpService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _otpService = otpService;
        }

        [HttpPost("request-otp")]
        public async Task<ActionResult<ApiResponse<string>>> RequestOtp([FromBody] OtpRequestDto request)
        {
            try
            {
                if (!ValidationHelper.IsEmail(request.Identifier) && !ValidationHelper.IsValidPhone(request.Identifier))
                {
                    return BadRequest(ApiResponse<string>.ErrorResponse("Please enter a valid email or phone number"));
                }

                var otp = await _otpService.GenerateOtpAsync(request.Identifier);
                await _otpService.SendOtpAsync(request.Identifier, otp);

                return Ok(ApiResponse<string>.SuccessResponse("", "OTP sent successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Signup([FromBody] VerifyOtpRequestDto request)
        {
            try
            {
                if (!ValidationHelper.IsValidRole(request.Role))
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid role"));
                }

                if (request.Role == "DeliveryPartner" && string.IsNullOrWhiteSpace(request.Pincode))
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Pincode is required for delivery partners"));
                }

                // Validate OTP
                var isOtpValid = await _otpService.ValidateOtpAsync(request.Identifier, request.Otp);
                if (!isOtpValid)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid or expired OTP"));
                }

                // Check if user already exists
                var existingUser = await _userRepository.GetAsync(u =>
                    u.Email == request.Identifier || u.Phone == request.Identifier);

                if (existingUser != null)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("User already exists"));
                }

                var user = new User
                {
                    Name = request.Name ?? "",
                    Email = ValidationHelper.IsEmail(request.Identifier) ? request.Identifier : null,
                    Phone = ValidationHelper.IsEmail(request.Identifier) ? null : request.Identifier,
                    PasswordHash = PasswordHelper.HashPassword(request.Password),
                    Role = request.Role,
                    Pincode = request.Pincode
                };

                await _userRepository.AddAsync(user);

                var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role,
                        Pincode = user.Pincode
                    },
                    Message = "Signup successful",
                    Success = true
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Signup successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Signup failed"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetAsync(u =>
                    u.Email == request.Identifier || u.Phone == request.Identifier);

                if (user == null)
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid credentials"));
                }

                if (!PasswordHelper.VerifyPassword(request.Password, user.PasswordHash))
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid credentials"));
                }

                var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role,
                        Pincode = user.Pincode
                    },
                    Message = "Login successful",
                    Success = true
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Login successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Login failed"));
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ApiResponse<string>>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                var user = await _userRepository.GetAsync(u =>
                    u.Email == request.Identifier || u.Phone == request.Identifier);

                if (user == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("User not found"));
                }

                var otp = await _otpService.GenerateOtpAsync(request.Identifier);
                await _otpService.SendOtpAsync(request.Identifier, otp);

                return Ok(ApiResponse<string>.SuccessResponse("", "OTP sent for password reset"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Failed to send OTP"));
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                var isOtpValid = await _otpService.ValidateOtpAsync(request.Identifier, request.Otp);
                if (!isOtpValid)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid or expired OTP"));
                }

                var user = await _userRepository.GetAsync(u =>
                    u.Email == request.Identifier || u.Phone == request.Identifier);

                if (user == null)
                {
                    return NotFound(ApiResponse<AuthResponseDto>.ErrorResponse("User not found"));
                }

                user.PasswordHash = PasswordHelper.HashPassword(request.NewPassword);
                user.UpdatedAt = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);

                // Revoke all existing refresh tokens
                // This could be implemented if needed

                var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role,
                        Pincode = user.Pincode
                    },
                    Message = "Password reset successful",
                    Success = true
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Password reset successful"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Password reset failed"));
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                var user = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken);
                if (user == null)
                {
                    return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid refresh token"));
                }

                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
                var (accessToken, refreshToken) = await _jwtService.GenerateTokensAsync(user);

                var response = new AuthResponseDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Phone = user.Phone,
                        Role = user.Role,
                        Pincode = user.Pincode
                    },
                    Message = "Token refreshed successfully",
                    Success = true
                };

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(response, "Token refreshed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("Token refresh failed"));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<string>>> Logout([FromBody] RefreshTokenRequestDto request)
        {
            try
            {
                await _jwtService.RevokeRefreshTokenAsync(request.RefreshToken);
                return Ok(ApiResponse<string>.SuccessResponse("", "Logged out successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse("Logout failed"));
            }
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
                }

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    Pincode = user.Pincode
                };

                return Ok(ApiResponse<UserDto>.SuccessResponse(userDto, "Profile retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("Failed to get profile"));
            }
        }
    }
}