using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCore.Dtos;
using NewCore.Services.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewCore.API.Controllers
{
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices customerServices;
        //private readonly ILogger<CustomerController> logger;

        public CustomerController(ICustomerServices _customerServices)
        //public CustomerController()
        {
            this.customerServices = _customerServices;
            //this.logger = _logger;
        }

        [HttpPost("GetCustomers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            //using CustomerServices cusServices = new CustomerServices();
            //try
            //{
            //    var results = (await customerServices.GetCustomersAsync()

            //    if (results.Count() == 0)
            //        return NotFound($"Not found Customer Record(s)");

            //    return new OkObjectResult(results);
            //}
            //catch (Exception ex)
            //{
            //    return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            //}
            try
            {
                //var results = (await customerServices.GetCustomersAsync()).Select(cus => cus.AsDto());
                var results = (await customerServices.GetCustomersAsync());

                if (results.Count() == 0)
                    return NotFound($"Not found Customer Record(s)");

                // logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {results.Count()} items");

                //return new OkObjectResult(results);
                return Ok(results);
            }
            catch (Exception ex)
            {
                //return BadRequest($"Error : {ex.Message}\nSource : {ex.Source}");
                //var err = StatusCode(StatusCodes.Status500InternalServerError, ex);
                //return err;
                //var errResult = new ObjectResult(ex.Message);
                //errResult.StatusCode = StatusCodes.Status500InternalServerError;
                //return errResult;
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("GetCustomer")]
        public async Task<ActionResult<CustomerDto>> GetCustomer([FromBody] CusIdDto req)
        {
            try
            {
                var result = await customerServices.GetCustomerAsync(req.customerId);
                if (result is null)
                    return NotFound($"Not found Customer Id = {req.customerId}");

                // logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved CustomerOD {cusId}");

                //return new OkObjectResult(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                //return BadRequest($"Error : {ex.Message}\nSource : {ex.Source}");
                //var err = StatusCode(StatusCodes.Status500InternalServerError, ex);
                //return err;
                //var errResult = new ObjectResult(ex.Message);
                //errResult.StatusCode = StatusCodes.Status500InternalServerError;
                //return errResult;
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("AddCustomer")]
        public async Task<ActionResult> AddCustomer([FromBody] CustomerDto cusDto)
        {
            try
            {
                var result = await customerServices.AddCustomerAsync(cusDto);
                if (result is null)
                    throw new ApplicationException($"Error Adding Customer ID = {cusDto.customerId}");
                // return with location and status 201
                return CreatedAtAction(nameof(GetCustomer), new { Id = result.CustomerId }, result);

            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("UpdateCustomer")]
        public async Task<ActionResult> UpdateCustomer([FromBody] CustomerDto cusDto)
        {
            try
            {
                await customerServices.UpdateCustomerAsync(cusDto);
                //var result = await customerServices.UpdateCustomerAsync(cusDto);
                //if (result is null)
                //    throw new ApplicationException($"Error Update Customer ID = {cusDto.customerId}");
                // return result with status 204
                //return new ObjectResult(result) { StatusCode = StatusCodes.Status204NoContent };
                return NoContent();
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }

        [HttpPost("DeleteCustomer")]
        public async Task<ActionResult> DeleteCustomer([FromBody] CusIdDto cusIdDto)
        {
            try
            {
                await customerServices.DeleteCustomerAsync(cusIdDto.customerId);
                //var result = await customerServices.UpdateCustomerAsync(cusDto);
                //if (result is null)
                //    throw new ApplicationException($"Error Update Customer ID = {cusDto.customerId}");
                // return result with status 204
                //return new ObjectResult(result) { StatusCode = StatusCodes.Status204NoContent };
                return NoContent();
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }
    }
}
