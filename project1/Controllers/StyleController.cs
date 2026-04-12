using Dto;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StyleController : ControllerBase
    {
        private readonly ILogger<StyleController> _logger;
        private readonly IStyleService _s;
        private readonly IUserServices _userService;

        public StyleController(IStyleService i, ILogger<StyleController> logger, IUserServices userService)
        {
            _s = i;
            _logger = logger;
            _userService = userService; 
        }

        // GET: api/<StyleController>
        [HttpGet]
        public async Task<IEnumerable<DtoSyle_id_name>> Get()
        {
            return await _s.GetStyles();
        }

        [HttpPost]
        public async Task<ActionResult<DtoSyle_id_name>> Post(
            [FromBody] DtoStyleAll StyleDto,
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: דרושות הרשאות מנהל להוספת סגנון");
            }

            DtoSyle_id_name res = await _s.AddNewStyle(StyleDto);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest("נכשל בהוספת הסגנון");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DtoSyle_id_name>> Delete(
            int id,
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: דרושות הרשאות מנהל למחיקת סגנון");
            }

            DtoSyle_id_name res = await _s.Delete(id);

            if (res != null)
            {
                return Ok(res);
            }
            return NotFound($"Style with ID {id} not found");
        }

        // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
        [HttpPost("upload")]
        public async Task<IActionResult> UploadStyleWithImage(
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

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "styles");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = $"{Guid.NewGuid()}_{image.FileName}";
                using (var stream = new FileStream(Path.Combine(uploadsFolder, fileName), FileMode.Create))
                    await image.CopyToAsync(stream);

                var styleDto = new DtoStyleAll(
                    name,
                    description,
                    $"/uploads/styles/{fileName}"
                );

                var result = await _s.AddNewStyle(styleDto);
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