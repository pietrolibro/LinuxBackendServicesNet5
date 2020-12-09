﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MyShopOnLine.Backend.Data;
using MyShopOnLine.Backend.Models;
using MyShopOnLine.Backend.Records;
using MyShopOnLine.Backend.Services;

namespace MyShopOnLine.Backend.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService customerService = null;

        public CustomersController(ICustomerService service)
        {
            this.customerService = service;
        }

        // GET: api/Customers
        /// <summary>
        /// Get all Customers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(CustomerRecord))]
        public async Task<ActionResult<IEnumerable<CustomerRecord>>> GetCustomers()
        {
            return await customerService.GetAsync();
        }

        // GET: api/Customers/5
        [HttpGet("{email}")]
        public async Task<ActionResult<CustomerRecord>> GetCustomer(string email)
        {
            var customer = await customerService.GetAsync(email);

            if (customer == null) return NotFound();

            return customer;
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{email}")]
        public async Task<IActionResult> PutCustomer(string email, CustomerRecord customer)
        {
            if (customer == null) return BadRequest("Customer is null");

            if (email != customer.Email) return BadRequest();

            var result = await customerService.UpdateAsync(email, customer);

            if (result.NotFound) return NotFound(result.ErrorMessage);

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerRecord customer)
        {
            var result = await customerService.CreateAsync(customer);

            if (result.AlreadyExists) return Conflict();

            if (!result.Success) return BadRequest(result.ErrorMessage);

            return CreatedAtAction("GetCustomer", new { email = customer.Email }, result.NewRecord);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteCustomer(string email)
        {
            var result = await customerService.RemoveAsync(email);

            if (result.NotFound) return NotFound();

            return NoContent();
        }
    }
}