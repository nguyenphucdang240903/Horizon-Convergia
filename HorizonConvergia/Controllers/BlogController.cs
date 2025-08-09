using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.DTO.ResultDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;
using System.Security.Claims;

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        //[HttpGet("category/{categoryId}")]
        //public async Task<IActionResult> GetByCategory(string categoryId)
        //{
        //    try
        //    {
        //        var blogs = await _blogService.GetByCategoryAsync(categoryId);
        //        return Ok(new ResultDTO
        //        {
        //            IsSuccess = true,
        //            Message = "Get blogs by category success.",
        //            Data = blogs
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ResultDTO
        //        {
        //            IsSuccess = false,
        //            Message = ex.Message,
        //            Data = null
        //        });
        //    }
        //}

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> GetById(string id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            return blog == null ? NotFound() : Ok(blog);
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateBlogDTO blogDto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Không thể xác định UserId từ token.",
                        Data = null
                    });
                }

                var blog = await _blogService.CreateMultipleAsync(blogDto, userId);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Create blog success.",
                    Data = blog // Có thể trả blog vừa tạo nếu muốn
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBlogDTO blogDto)
        {
            try
            {
                var userId = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ResultDTO
                    {
                        IsSuccess = false,
                        Message = "Không thể xác định UserId từ token.",
                        Data = null
                    });
                }

                var updated = await _blogService.UpdateAsync(id, blogDto, userId);

                return Ok(new ResultDTO
                {
                    IsSuccess = updated,
                    Message = updated ? "Update blog success." : "Blog not found.",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _blogService.DeleteAsync(id);
                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Delete blog success.",
                    Data = null
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}
