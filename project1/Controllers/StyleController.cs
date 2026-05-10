using Api.Auth;
using Dto;
using Microsoft.AspNetCore.Authorization;
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

        public StyleController(IStyleService i, ILogger<StyleController> logger)
        {
            _s = i;
            _logger = logger;
        }

        // GET: api/<StyleController>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<DtoSyle_id_name>> Get()
        {
            return await _s.GetStyles();
        }

        [AuthorizeRole("Admin")]
        [HttpPost]
        public async Task<ActionResult<DtoSyle_id_name>> Post([FromBody] DtoStyleAll StyleDto)
        {
            DtoSyle_id_name res = await _s.AddNewStyle(StyleDto);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest("נכשל בהוספת הסגנון");
        }

        [AuthorizeRole("Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<DtoSyle_id_name>> Delete(int id)
        {
            DtoSyle_id_name res = await _s.Delete(id);

            if (res != null)
            {
                return Ok(res);
            }
            return NotFound($"Style with ID {id} not found");
        }

        // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
        [AuthorizeRole("Admin")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadStyleWithImage()
        {
            try
            {
                var form = await Request.ReadFormAsync();
                var image = form.Files["image"];

                if (image == null)
                    return BadRequest(new { message = "חובה להעלות תמונה" });

                var name = form["name"].ToString();
                var description = form["description"].ToString();

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