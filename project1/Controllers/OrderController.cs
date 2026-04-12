using Dto;
using Microsoft.AspNetCore.Mvc;
using Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        IOrderService _s;
        public OrderController(IOrderService i)
        {
            _s = i;
        }



        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<DtoOrder_Id_UserId_Date_Sum_OrderItems>>> GetByUser(int userId)
        {
            var order = await _s.GetOrdersUser(userId);
            if (order != null)
            {
                return Ok(order);
            }
            return NoContent();
        }

        // GET api/<OrderController>/5
        // GET api/<users>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DtoOrder_Id_UserId_Date_Sum_OrderItems>> Get(int id)
        {
            DtoOrder_Id_UserId_Date_Sum_OrderItems order = await _s.GetOrderById(id);
            if (order != null)
            {
                return Ok(order);
            }
            return NoContent();
        }

        // POST api/<users>
        [HttpPost]
        public async Task<ActionResult<DtoOrder_Id_UserId_Date_Sum_OrderItems>> Post([FromBody] DtoOrder_Id_UserId_Date_Sum_OrderItems order)
        {

            DtoOrder_Id_UserId_Date_Sum_OrderItems res = await _s.AddNewOrder(order);
            if (res != null)
            {
                return CreatedAtAction(nameof(Get), new { id = res.OrderId }, res);
            }
            else
                return BadRequest();
        }

    }
}