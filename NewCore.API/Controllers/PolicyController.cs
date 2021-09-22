// using System;
using System.Collections.Generic;
using System.Threading.Tasks;
// using System.Linq;
// using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewCore.Data.Models;
using NewCore.Services.PolicyServices;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewCore.API.Controllers
{
    [Route("api/[controller]")]
    public class PolicyController : ControllerBase
    {
        private IPolicyServices policyServices;

        public PolicyController(IPolicyServices _policyServices)
        {
            this.policyServices = _policyServices;
        }

        [HttpGet("GetPolicies")]
        public async Task<IEnumerable<Policy>> GetPolicies()
        {
            return await policyServices.GetPoliciesAsync();
        }
    }
}
