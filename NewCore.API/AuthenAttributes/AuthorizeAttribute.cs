﻿using System;
using System.Collections.Generic;
using System.Linq;
using JWTIdentityClassLib.Entities;
using JWTIdentityClassLib.Enum;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[] roles)
    {
        _roles = roles ?? new Role[] { };
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        //var account = (Account)context.HttpContext.Items["Account"];
        //if (account != null && (!_roles.Any() || _roles.Contains(account.Role)))
        //{
        //    return;
        //}
        // not logged in or role not authorized
        context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        //
        var user = (ApplicationUser)context.HttpContext.Items["User"];
        if (user == null || (_roles.Any() && !_roles.Contains((Role)user.Role)))
        {
            // not logged in or role not authorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}