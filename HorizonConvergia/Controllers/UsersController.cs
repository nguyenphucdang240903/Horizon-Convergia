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
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> Get(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return user is not null ? Ok(user) : NotFound();
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(long id, UpdateUserDTO user)
        {
            user.Id = id;
            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> DeleteUser(long id)
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
