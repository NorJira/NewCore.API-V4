using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JWTIdentityClassLib.Enum;
using JWTIdentityClassLib.Services;
using JWTIdentityClassLib.Settings;
using JWTIdentityClassLib.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewCore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AuthBaseController
    {
        private IUserService _userService;
        private readonly JWTSettings _jwtSettings;
        private readonly ILogger<AuthController> _logger;


        public AuthController(IUserService userService,
            IOptions<JWTSettings> jwtSettings,
            ILogger<AuthController> logger)
        {
            this._userService = userService;
            this._jwtSettings = jwtSettings.Value;
            this._logger = logger;
        }

        // api/auth/Register
        [HttpPost("Register")]
        public async Task<ActionResult<UserManagerResponseDto>> RegisterAsync([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var result = await _userService.RegisterUserAsync(model, Request.Headers["origin"]);
                if (result.isSuccess)
                {
                    _logger.LogWarning($"Register new user {model.UserName}");
                    return Ok(result);
                }

                _logger.LogError(result.Message, result);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // api/auth/verify-email
        [HttpPost("verify-email")]
        public async Task<ActionResult<UserManagerResponseDto>> VerifyEmail(VerifyEmailRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var result = await _userService.VerifyEmailAsync(model.Token);
                if (result.isSuccess) return Ok(result);
                //
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // api/auth/authenticate
        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponseDto>> AuthenticateAsync(AuthenticateRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var response = await _userService.AuthenticateAsync(model, ipAddress());
                setTokenCookie(response.RefreshToken);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/forget-password
        [HttpPost("forget-password")]
        public async Task<ActionResult<UserManagerResponseDto>> ForgetPasswordAsync(ForgetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var result = await _userService.ForgetPasswordAsync(model, Request.Headers["origin"]);
                //return Ok(new { message = "Please check your email for password reset instructions" });
                if (result.isSuccess) return Ok(result);
                //
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<ActionResult<UserManagerResponseDto>> ResetPasswordAsync(ResetPasswordRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var result = await _userService.ResetPasswordAsync(model);
                //return Ok(new { message = "Please check your email for password reset instructions" });
                if (result.isSuccess) return Ok(result);
                //
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/changepassword
        [Authorize]
        [HttpPost("change-password")]
        public async Task<ActionResult<UserManagerResponseDto>> ChangePasswordAsync(ChangePasswordRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var result = await _userService.ChangePasswordAsync(model);
                //return Ok(new { message = "Please check your email for password reset instructions" });
                if (result.isSuccess) return Ok(result);
                //
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }


        // api/auth/revoke-token
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<ActionResult<UserManagerResponseDto>> RevokeTokenAsync(RevokeTokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                // accept token from request body or cookie
                var token = model.Token ?? Request.Cookies["refreshToken"];

                if (string.IsNullOrEmpty(token)) return BadRequest(new { Message = "Token is required" });
                //{
                //    return BadRequest(new UserManagerResponseDto {
                //        Message = "Token is required",
                //        isSuccess = false
                //    });
                //}

                // users can revoke their own tokens and admins can revoke any tokens
                if (!user.OwnsToken(token) && user.Role != (int)Role.Admin)
                    return base.Unauthorized(new { message = "Unauthorized" });

                var result = await _userService.RevokeTokenAsync(token, ipAddress());
                if (result.isSuccess) return Ok(result);
                //
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/refresh-token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponseDto>> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                    return BadRequest("Not found refresh token");
                //
                var response = await _userService.RefreshTokenAsync(refreshToken, ipAddress());
                //
                if (response != null) { 
                    setTokenCookie(response.RefreshToken);
                    return Ok(response);
                };
                return BadRequest("Error Refresh Token");
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/validate-reset-token
        [HttpPost("validate-reset-token")]
        public async Task<ActionResult<UserManagerResponseDto>> ValidateResetTokenAsync(ValidateResetTokenRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                var response = await _userService.ValidateResetTokenAsync(model);
                return Ok(new { message = "Token is valid" });
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/getusers
        [Authorize(Role.Admin)]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IList<UserResponseDto>>> GetUsersAsync()
        {
            try
            {
                var users = await _userService.GetUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/getuserbyemail
        [Authorize]
        [HttpPost("GetUserByEmail")]
        public async Task<ActionResult<UserResponseDto>> GetUserByEmailAsync(UserByEmailRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                // users can get their own account and admins can get any account
                if (model.Email != user.Email && user.Role != (int)Role.Admin)
                    return base.Unauthorized(new { message = "Unauthorized" });

                var response = await _userService.GetUserByEmailAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/getrefreshtokensbyid
        [Authorize]
        [HttpPost("GetRefreshTokensById")]
        public async Task<ActionResult<IList<UserResponseDto>>> GetRefreshTokensByIdAsync(RefreshTokensByUserIdRequestDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);  // ("Invalid input model!!!");
            //
            try
            {
                // users can get their own account and admins can get any account
                if (model.Id != user.Id && user.Role != (int)Role.Admin)
                    return base.Unauthorized(new { message = "Unauthorized" });

                var response = await _userService.GetRefreshTokensByUserIdAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }


        #region CRUD functions

        // api/auth/createuser
        [Authorize(Role.Admin)]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserResponseDto>> CreateUserAsync(RegisterRequestDto model)
        {
            try
            {
                var response = await _userService.CreateAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/updateuser
        [Authorize(Role.Admin)]
        [HttpPost("UpdateUser")]
        public async Task<ActionResult<UserResponseDto>> UpdateUserAsync(UpdateRequestDto model)
        {
            try
            {
                var response = await _userService.UpdateAsync(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        // api/auth/updateuser
        [Authorize(Role.Admin)]
        [HttpPost("DeleteUser")]
        public async Task<ActionResult<UserManagerResponseDto>> DeleteUserAsync(DeleteRequestDto model)
        {
            try
            {
                var response = await _userService.DeleteAsync(model.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                //return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
                return BadRequest(ex);
            }
        }

        #endregion


        #region Private Functions

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenTTL)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        #endregion

    }
}

//<properties>
//    <property key = 'SourceContext'>NewCore.API.Controllers.AuthController</property>
//    <property key = 'ActionId'>2f715463-dd38-4531-94c8-083d57508c6a</property>
//    <property key = 'ActionName' > NewCore.API.Controllers.AuthController.RegisterAsync(NewCore.API)</property>
//    <property key ='RequestId'>0HMCOCKO6T4C0:00000002</property>
//    <property key = 'RequestPath' >/ api / auth / register </ property >
//    < property key='ConnectionId'>0HMCOCKO6T4C0</property>
//</properties>
