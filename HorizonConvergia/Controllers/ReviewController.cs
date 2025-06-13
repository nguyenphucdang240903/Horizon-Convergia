using BusinessObjects.DTO.BlogDTO;
using BusinessObjects.DTO.ResultDTO;
using BusinessObjects.DTO.ReviewDTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace HorizonConvergia.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var reviews = await _reviewService.GetAllAsync();
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            return review == null ? NotFound() : Ok(review);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDTO reviewDto)
        {
            try
            {
                var blog = await _reviewService.CreateAsync(reviewDto);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Create Review Success.",
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
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReviewDTO reviewDto)
        {
            try
            {
                var blog = await _reviewService.UpdateAsync(id, reviewDto);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Update Review Success.",
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

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(string id, [FromBody] DeleteReviewDTO reviewDto)
        //{
        //    try
        //    {
        //        var success = await _reviewService.DeleteAsync(id, reviewDto);
        //        return Ok(new ResultDTO
        //        {
        //            IsSuccess = true,
        //            Message = "Delete Review Success.",
        //            Data = null
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
    }
}
