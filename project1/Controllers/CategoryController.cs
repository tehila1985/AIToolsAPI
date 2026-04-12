using Dto;
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
        private readonly IUserServices _userService; 

        public CategoryController(ICategoryService i, ILogger<CategoryController> logger, IUserServices userService)
        {
            _s = i;
            _logger = logger;
            _userService = userService;
        }

        // GET: api/<CategoryController>
        [HttpGet]
        public async Task<IEnumerable<DtoCategory_Name_Id>> Get()
        {
            return await _s.GetCategories();
        }

        [HttpPost]
        public async Task<ActionResult<DtoCategory_Name_Id>> Post(
            [FromBody] DtocategoryAll categoryDto, 
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: פעולה זו שמורה למנהלים בלבד");
            }

            DtoCategory_Name_Id res = await _s.AddNewCategory(categoryDto);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest("נכשל בהוספת הקטגוריה");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DtoCategory_Name_Id>> Delete(
            int id,
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: מחיקת קטגוריה דורשת הרשאות מנהל");
            }

            DtoCategory_Name_Id res = await _s.Delete(id);

            if (res != null)
            {
                return Ok(res);
            }
            return NotFound($"Category with ID {id} not found");
        }

        // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCategoryWithImage(
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var image = form.Files["image"];

                if (image == null)
                    return BadRequest(new { message = "חובה להעלות תמונה" });

                var name = form["name"].ToString();
                var description = form["description"].ToString();

                bool isAdmin = await _userService.IsAdminById(userId, password);
                if (!isAdmin)
                    return StatusCode(403, new { message = "אין הרשאות מנהל" });

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