using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using MyShopOnLine.Backend.Services;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Models;

namespace MyShopOnLine.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService productService = null;

        public ProductsController(IProductService service)
        {
            this.productService = service;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRecord>>> GetProducts()
        {
            return await productService.GetAsync();
        }

        // GET: api/Products/5
        [HttpGet("{code}")]
        public async Task<ActionResult<ProductRecord>> GetProduct(string code)
        {
            var product = await productService.GetAsync(code);

            if (product == null) return NotFound();

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{code}")]
        public async Task<IActionResult> PutProduct(string code, ProductRecord product)
        {
            if (product == null) return BadRequest("Product is null");

            if (code != product.Code) return BadRequest();

            var result = await productService.UpdateAsync(code, product);

            if (result.NotFound) return NotFound(result.ErrorMessage);

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductRecord product)
        {
            var result = await productService.CreateAsync(product);

            if (result.AlreadyExists) return Conflict();

            if (!result.Success) return BadRequest(result.ErrorMessage);

            return CreatedAtAction("GetProduct", new { code = product.Code }, product);
        }

        // DELETE: api/Products/5
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteProduct(string code)
        {
            var result = await productService.RemoveAsync(code);

            if (result.NotFound) return NotFound();

            return NoContent();
        }
    }
}
