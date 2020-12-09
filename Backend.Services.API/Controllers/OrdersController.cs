using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services;

namespace MyShopOnLine.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService = null;

        public OrdersController(IOrderService service)
        {
            this.orderService = service;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderRecord>>> GetOrders()
        {
            return await orderService.GetAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{number}")]
        public async Task<ActionResult<OrderRecord>> GetOrder(string number)
        {
            var order = await orderService.GetAsync(number);

            if (order == null) return NotFound();

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{number}")]
        public async Task<IActionResult> PutOrder(string number, OrderRecord order)
        {
            if (order == null) return BadRequest("Order is null");

            if (number != order.Number) return BadRequest();

            var result = await orderService.UpdateAsync(number, order);

            if (result.NotFound) return NotFound(result.ErrorMessage);

            return NoContent();
        }


        //// POST: api/Orders
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost("test-new-order")]
        //public async Task<ActionResult<OrderRecord>> TestOrder()
        //{
        //    List<OrderEntry> entries = new List<OrderEntry>();

        //    entries.Add(new OrderEntry("ITEM00001", 2));
        //    entries.Add(new OrderEntry("ITEM00002", 1));

        //    OrderRecord orderIn = new OrderRecord("pietro.libro@gmail.com", entries);

        //    CreateResult<OrderRecord> result = await orderService.CreateAsync(orderIn);

        //    if (result.Success) return result.NewEntity;

        //    return BadRequest(result.ErrorMessage);
        //}

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderRecord order)
        {
            var result = await orderService.CreateAsync(order);

            if (result.AlreadyExists) return Conflict();

            if (!result.Success) return BadRequest(result.ErrorMessage);

            return CreatedAtAction("GetOrder", new { number = order.Number }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{number}")]
        public async Task<IActionResult> DeleteOrder(string number)
        {
            var result = await orderService.RemoveAsync(number);

            if (result.NotFound) return NotFound();

            return NoContent();
        }
    }
}
