using BusinessObjects.DTO.ImageDTO;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HorizonConvergia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImagesService _imagesService;
        public ImagesController(IImagesService imagesService)
        {
            _imagesService = imagesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var images = await _imagesService.GetAllAsync();
            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var image = await _imagesService.GetByIdAsync(id);
            if (image == null) return NotFound();
            return Ok(image);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateImagesDTO dto)
        {
            var image = await _imagesService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = image.Id }, image);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateImagesDTO dto)
        {
            var updated = await _imagesService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _imagesService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
