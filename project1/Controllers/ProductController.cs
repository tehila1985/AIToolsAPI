using Dto;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Collections.Generic;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IProductService _s;
        private readonly IUserServices _userService; 

        public ProductController(IProductService i, ILogger<ProductController> logger, IUserServices userService)
        {
            _s = i;
            _logger = logger;
            _userService = userService; 
        }

        [HttpGet]
        public async Task<Dto_result_product> Gets(
            [FromQuery] int position,
            [FromQuery] int skip,
            [FromQuery] string? desc,
            [FromQuery] int? minPrice,
            [FromQuery] int? maxPrice,
            [FromQuery] int?[] categoryIds,
            [FromQuery] int?[] styleIds)
        {
            return await _s.GetProducts(position, skip, desc, minPrice, maxPrice, categoryIds, styleIds);
        }

        [HttpPost]
        public async Task<ActionResult<DtoProduct_Id_Name_Category_Price_Desc_Image>> Post(
            [FromBody] DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds productDto, 
            [FromHeader] int userId,  
            [FromHeader] string password)
        {
            
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: דרושות הרשאות מנהל לביצוע פעולה זו");
            }

            DtoProduct_Id_Name_Category_Price_Desc_Image res = await _s.AddNewProduct(productDto);
            if (res != null)
            {
                return Ok(res);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<DtoProduct_Id_Name_Category_Price_Desc_Image>> Delete(
            int id,
            [FromHeader] int userId,   
            [FromHeader] string password)
        {
           
            bool isAdmin = await _userService.IsAdminById(userId, password);
            if (!isAdmin)
            {
                return Forbid("גישה נדחתה: דרושות הרשאות מנהל למחיקת מוצר");
            }

            DtoProduct_Id_Name_Category_Price_Desc_Image res = await _s.Delete(id);

            if (res != null)
            {
                return Ok(res);
            }
            return NotFound($"Product with ID {id} not found");
        }
        [HttpGet("{id}")]
        public async Task<DtoProduct_Id_Name_Category_Price_Desc_Image> Get(int id)
        {
            return await _s.GetById(id);
        }

        // ===== נוסף עבור הקריאה ל-AI - מחזיר מוצרים לפי רשימת IDs =====
        [HttpGet("by-ids")]
        public async Task<List<DtoProduct_Id_Name_Category_Price_Desc_Image>> GetByIds([FromQuery] int[] ids)
        {
            var products = new List<DtoProduct_Id_Name_Category_Price_Desc_Image>();
            
            foreach (var id in ids)
            {
                try
                {
                    var product = await _s.GetById(id);
                    if (product != null)
                        products.Add(product);
                }
                catch
                {
                    // אם מוצר לא נמצא, פשוט נדלג עליו
                }
            }
            
            return products;
        }

        // ===== נוסף עבור העלאת תמונות למנהל - התחלה =====
        [HttpPost("upload")]
        public async Task<IActionResult> UploadProductWithImages(
            [FromHeader] int userId,
            [FromHeader] string password)
        {
            try
            {
                var form = await Request.ReadFormAsync();

                var frontImage = form.Files["frontImage"];
                var backImage = form.Files["backImage"];

                if (frontImage == null || backImage == null)
                    return BadRequest(new { message = "חובה להעלות שתי תמונות" });

                var name = form["name"].ToString();
                var description = form["description"].ToString();
                var price = decimal.Parse(form["price"].ToString());
                var categoryId = int.Parse(form["categoryId"].ToString());

                bool isAdmin = await _userService.IsAdminById(userId, password);
                if (!isAdmin)
                    return StatusCode(403, new { message = "אין הרשאות מנהל" });

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "products");
                Directory.CreateDirectory(uploadsFolder);

                var frontFileName = $"{Guid.NewGuid()}_{frontImage.FileName}";
                var backFileName = $"{Guid.NewGuid()}_{backImage.FileName}";

                using (var stream = new FileStream(Path.Combine(uploadsFolder, frontFileName), FileMode.Create))
                    await frontImage.CopyToAsync(stream);

                using (var stream = new FileStream(Path.Combine(uploadsFolder, backFileName), FileMode.Create))
                    await backImage.CopyToAsync(stream);

                var productDto = new DtoProduct_Name_Description_Price_Stock_CategoryId_IsActive_StyleIds(
                    name,
                    description,
                    price,
                    $"/uploads/products/{frontFileName}",
                    $"/uploads/products/{backFileName}",
                    new List<DtoSyle_id_name>(),
                    0,
                    categoryId,
                    true
                );

                var result = await _s.AddNewProduct(productDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "שגיאה: {Message}", ex.Message);
                return StatusCode(500, new { message = "שגיאה בהעלאת התמונות", error = ex.Message });
            }
        }
        // ===== נוסף עבור העלאת תמונות למנהל - סוף =====
    }
}