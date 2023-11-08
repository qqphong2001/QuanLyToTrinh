
using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppRoleController : ControllerBase
    {
        private readonly IRoleSerice _RoleService;
        public AppRoleController(IRoleSerice RoleService)
        {
            _RoleService = RoleService;
        }

      
      
        [HttpGet("GetAllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                var result = await _RoleService.GetAllRoles();
                return Ok(new BaseResponseModel(result));
            }
            catch(Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}
