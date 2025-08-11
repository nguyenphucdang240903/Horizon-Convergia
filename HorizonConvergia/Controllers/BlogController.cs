using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.DTO.ProductDTO;
using BusinessObjects.DTO.ResultDTO;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interfaces;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var blogs = await _blogService.GetAllAsync();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var blog = await _blogService.GetByIdAsync(id);
            return blog == null ? NotFound() : Ok(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlogDTO blogDto)
        {
            try
            {
                var blog = await _blogService.CreateMultipleAsync(blogDto);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Create blog success.",
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateBlogDTO blogDto)
        {
            try
            {
                var blog = await _blogService.UpdateAsync(id, blogDto);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Update blog success.",
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
