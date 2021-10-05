using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewCore.Dtos;
using NewCore.Services.PolicyServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewCore.API.Controllers
{
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private IPolicyServices policyServices;
        //private ILogger<PolicyController> logger;

        //public PolicyController(IPolicyServices _policyServices, ILogger<PolicyController> _logger)
        public PolicyController(IPolicyServices _policyServices)
        {
            this.policyServices = _policyServices;
            //this.logger = _logger;
        }

        [HttpPost("GetPolicies")]
        public async Task<IEnumerable<PolicyDto>> GetPolicies()
        {
            var results = await policyServices.GetPoliciesAsync();

            //logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {results.Count()} items");

            return results;
        }

        [HttpPost("GetPolicy")]
        public async Task<ActionResult<PolicyDto>> GetPolicy([FromBody] PolIdDto req)
        {
            try
            {
                var result = await policyServices.GetPolicyAsync(req.polId);


                if (result is null)
                    return NotFound($"Not found Policy Id = {req.polId}");

                // logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved PolicyOD {cusId}");

                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("AddPolicy")]
        public async Task<ActionResult> AddPolicy([FromBody] PolicyDto polDto)
        {
            try
            {
                var result = await policyServices.AddPolicyAsync(polDto);
                if (result is null)
                    throw new ApplicationException($"Error Adding Policy ID = {polDto.polId}");
                // return with location and status 201
                return CreatedAtAction(nameof(GetPolicy), new { Id = result.PolId }, result);

            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("UpdatePolicy")]
        public async Task<ActionResult> UpdatePolicy([FromBody] PolicyDto polDto)
        {
            try
            {
                await policyServices.UpdatePolicyAsync(polDto);
                
                return NoContent();
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }

        [HttpPost("DeletePolicy")]
        public async Task<ActionResult> DeletePolicy([FromBody] PolIdDto polIdDto)
        {
            try
            {
                await policyServices.DeletePolicyAsync(polIdDto.polId);
               
                return NoContent();
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }

    }
}
