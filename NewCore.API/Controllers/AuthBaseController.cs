using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JWTIdentityClassLib.Entities;
using Microsoft.AspNetCore.Mvc;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NewCore.API.Controllers
{
    [Controller]
    public abstract class AuthBaseController : ControllerBase
    {
        // returns the current authenticated account (null if not logged in)
        public ApplicationUser user => (ApplicationUser)HttpContext.Items["User"];
    }
}

