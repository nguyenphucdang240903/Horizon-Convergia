using BusinessObjects.DTO.ResultDTO;
using BusinessObjects.DTO.UserDTO;
using BusinessObjects.Models;
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

       

        [HttpGet("{id}")]
        [Authorize(Policy = "Buyer")]

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
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound($"User with ID {id} not found.");
            }

            return Ok(new { message = $"User with ID {id} deleted successfully." });
        }
    }
}
