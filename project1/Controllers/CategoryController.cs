using Api.Auth;
using Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _s;

        public CategoryController(ICategoryService i, ILogger<CategoryController> logger)
        {
            _s = i;
            _logger = logger;
        }

        // GET: api/<CategoryController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<DtoCategory_Name_Id>> Get()
        {
            return await _s.GetCategories();
        }

        [AuthorizeRole("Admin")]
        [HttpPost]
        public async Task<ActionResult<DtoCategory_Name_Id>> Post([FromBody] DtocategoryAll categoryDto)
        {
            DtoCategory_Name_Id res = await _s.AddNewCategory(categoryDto);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest("נכשל בהוספת הקטגוריה");
        }

        [AuthorizeRole("Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DtoCategory_Name_Id>> Delete(int id)
        {
            DtoCategory_Name_Id res = await _s.Delete(id);

            if (res != null)
            {
                return Ok(res);
            }
            return NotFound($"Category with ID {id} not found");
        }

        // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
        [AuthorizeRole("Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCategoryWithImage()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var image = form.Files["image"];

                if (image == null)
                    return BadRequest(new { message = "חובה להעלות תמונה" });

                var name = form["name"].ToString();
                var description = form["description"].ToString();

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "categories");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                using (var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create))
                    await image.CopyToAsync(stream);
                var categoryDto = new DtocategoryAll(
                    name,
                    description,
                    $"/uploads/categories/{fileName}"
                );

                var result = await _s.AddNewCategory(categoryDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה: {Message}", ex.Message);
                return StatusCode(500, new { message = "שגיאה בהעלאת התמונה", error = ex.Message });
            }
        }
        // ===== נוסף עבור העלאת תמונות למנהל - סוף =====
    }
}