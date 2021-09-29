using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewCore.Dtos;
using NewCore.Services.PolCvgServices;
//using NewCore.Services.PolCvgServices;

namespace NewCore.API.Controllers
{
    [Route("api/[controller]")]
    public class PolCvgController : ControllerBase
    {
        private IPolCvgServices polcvgServices;
        //private ILogger<PolCvgController> logger;

        public PolCvgController(IPolCvgServices _polcvgServices)
        {
            this.polcvgServices = _polcvgServices;
            //this.logger = _logger;
        }

        [HttpPost("GetPolCvgByPolId")]
        public async Task<ActionResult<PolCvgDto>> GetPolCvgByPolId([FromBody] PolIdDto polId)
        {
            try
            {
                var results = await polcvgServices.GetPolCvgByPolIdAsync(polId.polId);

                //logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {results.Count()} items");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }
        }

        [HttpPost("AddPolCvgs")]
        public async Task<ActionResult<PolCvgDto>> AddPolCvgs([FromBody] PolCvgDto polCvgDto)
        {
            try
            {
                var results = await polcvgServices.AddPolCvgsAsyc(polCvgDto);

                //logger.LogInformation($"{DateTime.UtcNow.ToString("hh:mm:ss")}: Retrieved {results.Count()} items");

                return Ok(results);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.Message) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            
        }
    }
}
