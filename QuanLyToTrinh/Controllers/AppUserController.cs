using Aspose.Pdf.Operators;
using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUserService _userService;
        public AppUserController(IAppUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LogInDTO payload)
        {
            try
            {
                var result = await _userService.UserLogin(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpDTO payload)
        {
            try
            {
                var result = await _userService.UserSignUp(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch(Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassWord(ChangePasswordDTO payload)
        {
            try
            {
                var result = await _userService.ChangePassword(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {             
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpPut("ResetPassword/{userId}")]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            try
            {
                var result = await _userService.ResetPassword(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }

        [HttpGet("GetAllUserInfo")]
        public async Task<IActionResult> GetAllUserInfo()
        {
            try
            {
                var result = await _userService.GetAllInfoUser();
                return Ok(new BaseResponseModel(result));
            }
            catch(Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
