using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services;
using System;

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
        [ProducesResponseType(200,Type=typeof(OrderRecord))]
        public async Task<ActionResult<IEnumerable<OrderRecord>>> GetOrders()
        {
            return await orderService.GetAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{number}")]
        [ProducesResponseType(200,Type=typeof(OrderRecord))]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrderRecord>> GetOrder(string number)
        {
            var order = await orderService.GetAsync(number);

            if (order == null) return NotFound();

            return order;
        }

        // PUT: api/Orders/5
        [HttpPut("{number}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutOrder(string number, OrderRecord order)
        {
            if (order == null) return BadRequest("Order is null");

            if (number != order.Number) return BadRequest();

            var result = await orderService.UpdateAsync(number, order);

            if (result.NotFound) return NotFound(result.ErrorMessage);

            return NoContent();
        }       

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        public async Task<ActionResult<Order>> PostOrder(OrderRecord order)
        {
            var result = await orderService.CreateAsync(order);

            if (result.AlreadyExists) return Conflict();

            if (!result.Success) return BadRequest(result.ErrorMessage);

            return CreatedAtAction("GetOrder", new { number = order.Number }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{number}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteOrder(string number)
        {
            var result = await orderService.RemoveAsync(number);

            if (result.NotFound) return NotFound();

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost("create-new-random-order")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OrderRecord>> TestOrder()
        {
            Random rnd = new Random();

            List<OrderEntry> entries = new List<OrderEntry>();

            string[] productCodes = new string[]
            {
                "ITEM00001","ITEM00002","ITEM00003"
            };    

           entries.Add(new OrderEntry(productCodes[rnd.Next(0,1)], rnd.Next(1,5)));
           entries.Add(new OrderEntry(productCodes[2], rnd.Next(1,5)));

           OrderRecord orderIn = new OrderRecord("pietro.libro@gmail.com", entries);

            var result = await orderService.CreateAsync(orderIn);

           if (result.Success) return result.NewRecord;

           return BadRequest(result.ErrorMessage);
        }
    }
}
