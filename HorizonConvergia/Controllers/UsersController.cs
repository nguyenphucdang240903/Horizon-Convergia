using BusinessObjects.DTO.ResultDTO;
using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService) => _userService = userService;

        [HttpPost("admin-create")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AdminCreateUser([FromBody] CreateUserByAdminDTO dto)
        {
            var user = await _userService.AdminCreateUserAsync(dto);
            return Ok(new ResultDTO
            {
                IsSuccess = true,
                Message = "Tạo tài khoản thành công, vui lòng kiểm tra email để xác thực.",
                Data = new
                {
                    user.Id,
                    user.Email,
                    user.Role
                }
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user is not null)
            {
                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Lấy thông tin người dùng thành công.",
                    Data = user
                });
            }
            return NotFound(new ResultDTO
            {
                IsSuccess = false,
                Message = $"Không tìm thấy người dùng với ID: {id}.",
                Data = null
            });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, UpdateUserDTO user)
        {
            user.Id = id;
            await _userService.UpdateUserAsync(user);
            return Ok(new ResultDTO
            {
                IsSuccess = true,
                Message = "Update thành công",
                Data = user
            });

        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound($"Đéo tìm thấy người dùng có ID {id}");
            }

            return Ok(new { message = $"Người dùng với ID {id} xóa thành công." });
        }
        [HttpGet("search")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> SearchUsers(
     [FromQuery] string? keyword,
     [FromQuery] UserRole? role,
     [FromQuery] UserStatus? status,
     [FromQuery] int pageIndex = 1,
     [FromQuery] int pageSize = 10,
     [FromQuery] string sortBy = "CreatedAt",
     [FromQuery] string sortOrder = "desc")
        {
            var users = await _userService.SearchUsersAsync(keyword, role, status, pageIndex, pageSize, sortBy, sortOrder);
            var total = await _userService.CountSearchUsersAsync(keyword, role, status);

            return Ok(new ResultDTO
            {
                IsSuccess = true,
                Message = "Tìm kiếm người dùng thành công.",
                Data = new PagedResultDTO<UserBasicDTO>
                {
                    Items = users,
                    TotalRecords = total,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                }
            });
        }


    }
}
