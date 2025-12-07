using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using UAParser;
using UserService.API.DTOs;
using UserService.Application.DTOs;
using UserService.Application.Services;

namespace UserService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpPost("register")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            try
            {
                var result = await _userService.RegisterAsync(dto);
                if (!result)
                    return BadRequest(APIResponse<string>.FailResponse("Registration failed. Email or username might already exist."));

                return Ok(APIResponse<string>.SuccessResponse("User registered successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error during registration.", new List<string> { ex.Message }));
            }
        }


        [HttpPost("send-confirmation-email")]
        [ProducesResponseType(typeof(APIResponse<EmailConfirmationTokenResponseDTO>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] EmailDTO dto)
        {
            try
            {
                var emailTokenResponse = await _userService.SendConfirmationEmailAsync(dto.Email);
                if (emailTokenResponse == null)
                    return NotFound(APIResponse<string>.FailResponse("User with this email not found"));

                return Ok(APIResponse<EmailConfirmationTokenResponseDTO>.SuccessResponse(emailTokenResponse, "Email confirmation token generated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error generating confirmation token.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("verify-email")]
        [ProducesResponseType(typeof(APIResponse<string>), 200)]
        [ProducesResponseType(typeof(APIResponse<string>), 400)]
        public async Task<IActionResult> VerifyConfirmationEmailAsync([FromBody] ConfirmEmailDTO dto)
        {
            try
            {
                var success = await _userService.VerifyConfirmationEmailAsync(dto);
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Invalid confirmation token or user."));

                return Ok(APIResponse<string>.SuccessResponse("Email confirmed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error confirming email.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("login1")]
        public async Task<IActionResult> Login1([FromBody] LoginDTO dto)
        {
            var IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var UserAgent = GetNormalizedUserAgent();
            var loginResponse = await _userService.LoginAsync(dto, IPAddress, UserAgent);

            // Always return LoginResponseDTO wrapped in APIResponse
            if (!string.IsNullOrEmpty(loginResponse.ErrorMessage))
            {
                // Failure case - Success = false, return DTO with error message
                loginResponse.Succeeded = false; // Add this property if missing
                return Unauthorized(APIResponse<LoginResponseDTO>.FailResponse(loginResponse.ErrorMessage, errors: null, data: loginResponse));
            }

            // Success or requires 2FA
            loginResponse.Succeeded = true; // Make sure this is set on success path as well
            return Ok(APIResponse<LoginResponseDTO>.SuccessResponse(loginResponse,
                loginResponse.RequiresTwoFactor ? "Two-factor authentication required." : "Login successful."));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var userAgent = GetNormalizedUserAgent();

            _logger.LogInformation($"Login request received. IP={ipAddress}, EmailOrUserName={dto.EmailOrUserName}");

            try
            {
                var loginResponse = await _userService.LoginAsync(dto, ipAddress, userAgent);

                if (!string.IsNullOrEmpty(loginResponse.ErrorMessage))
                {
                    _logger.LogWarning($"Login failed for {dto.EmailOrUserName}. Reason: {loginResponse.ErrorMessage}");

                    loginResponse.Succeeded = false;
                    return Unauthorized(APIResponse<LoginResponseDTO>.FailResponse(
                        loginResponse.ErrorMessage, null, loginResponse));
                }

                loginResponse.Succeeded = true;
                _logger.LogInformation(loginResponse.RequiresTwoFactor
                        ? $"2FA required for {dto.EmailOrUserName}"
                        : $"User {dto.EmailOrUserName} logged in successfully.");

                return Ok(APIResponse<LoginResponseDTO>.SuccessResponse(
                    loginResponse,
                    loginResponse.RequiresTwoFactor
                        ? "Two-factor authentication required."
                        : "Login successful."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred during login for {dto.EmailOrUserName}");
                return StatusCode(500, APIResponse<LoginResponseDTO>.FailResponse($"Unexpected error occurred during login for {dto.EmailOrUserName}", new List<string> { ex.Message }));
            }
        }



        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
        {
            var IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";
            var UserAgent = GetNormalizedUserAgent();

            var refreshTokenResponse = await _userService.RefreshTokenAsync(dto, IPAddress, UserAgent);

            if (!string.IsNullOrEmpty(refreshTokenResponse.ErrorMessage))
                return Unauthorized(APIResponse<string>.FailResponse(refreshTokenResponse.ErrorMessage));

            return Ok(APIResponse<RefreshTokenResponseDTO>.SuccessResponse(refreshTokenResponse, "Token refreshed successfully."));
        }

        [HttpPost("revoke-token")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequestDTO dto)
        {
            try
            {
                var success = await _userService.RevokeRefreshTokenAsync(dto.RefreshToken, HttpContext.Connection.RemoteIpAddress?.ToString() ?? "");
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Invalid token or token already revoked."));

                return Ok(APIResponse<string>.SuccessResponse("Token revoked successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error revoking token.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("profile/{userId}")]
        [ProducesResponseType(typeof(APIResponse<ProfileDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProfile(Guid userId)
        {
            try
            {
                var profile = await _userService.GetProfileAsync(userId);
                if (profile == null)
                    return NotFound(APIResponse<string>.FailResponse("User profile not found."));

                return Ok(APIResponse<ProfileDTO>.SuccessResponse(profile));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error fetching profile.", new List<string> { ex.Message }));
            }
        }

        [HttpPut("profile")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            try
            {
                var success = await _userService.UpdateProfileAsync(dto);
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Failed to update profile."));

                return Ok(APIResponse<string>.SuccessResponse("Profile updated successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error updating profile.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDTO dto)
        {
            try
            {
                var forgotPassword = await _userService.ForgotPasswordAsync(dto.Email);
                if (forgotPassword == null)
                    return NotFound(APIResponse<string>.FailResponse("Email not found."));

                return Ok(APIResponse<ForgotPasswordResponseDTO>.SuccessResponse(forgotPassword, "Password reset token sent to email."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error in forgot password process.", new List<string> { ex.Message }));
            }
        }

        // Reset Password (Forgot Password Flow)
        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(APIResponse<string>), 200)]
        [ProducesResponseType(typeof(APIResponse<string>), 400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            try
            {
                var success = await _userService.ResetPasswordAsync(dto.UserId, dto.Token, dto.NewPassword);
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Invalid token or user."));

                return Ok(APIResponse<string>.SuccessResponse("Password reset successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error resetting password.", new List<string> { ex.Message }));
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(typeof(APIResponse<string>), 200)]
        [ProducesResponseType(typeof(APIResponse<string>), 400)]
        [ProducesResponseType(typeof(APIResponse<string>), 401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                    return Unauthorized(APIResponse<string>.FailResponse("Invalid user token."));

                var success = await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Password change failed."));

                return Ok(APIResponse<string>.SuccessResponse("Password changed successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error changing password.", new List<string> { ex.Message }));
            }
        }

        [HttpPost("addresses")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> AddOrUpdateAddress([FromBody] AddressDTO dto)
        {
            try
            {
                var success = await _userService.AddOrUpdateAddressAsync(dto);
                if (!success)
                    return BadRequest(APIResponse<string>.FailResponse("Failed to add or update address."));

                return Ok(APIResponse<string>.SuccessResponse("Address saved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error saving address.", new List<string> { ex.Message }));
            }
        }

        [HttpGet("{userId}/addresses")]
        [ProducesResponseType(typeof(APIResponse<IEnumerable<AddressDTO>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAddresses(Guid userId)
        {
            try
            {
                var addresses = await _userService.GetAddressesAsync(userId);
                return Ok(APIResponse<IEnumerable<AddressDTO>>.SuccessResponse(addresses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error fetching addresses.", new List<string> { ex.Message }));
            }
        }

        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.NotFound)]
        [HttpPost("delete-address")]
        public async Task<IActionResult> DeleteAddress([FromBody] DeleteAddressDTO dto)
        {
            try
            {
                var deleted = await _userService.DeleteAddressAsync(dto.UserId, dto.AddressId);
                if (!deleted)
                    return BadRequest(APIResponse<string>.FailResponse("Address not found or deletion failed."));

                return Ok(APIResponse<string>.SuccessResponse("Address deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.FailResponse("Error deleting address.", new List<string> { ex.Message }));
            }
        }

        private string GetNormalizedUserAgent()
        {
            var userAgentRaw = HttpContext.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrWhiteSpace(userAgentRaw))
                return "Unknown";

            try
            {
                var uaParser = Parser.GetDefault();
                ClientInfo clientInfo = uaParser.Parse(userAgentRaw);

                var browser = clientInfo.UA.Family ?? "UnknownBrowser";
                var browserVersion = clientInfo.UA.Major ?? "0";
                var os = clientInfo.OS.Family ?? "UnknownOS";

                return $"{browser}-{browserVersion}_{os}";
            }
            catch
            {
                // In case parsing fails, fallback to raw user agent or unknown
                return "Unknown";
            }
        }
    }
}



