﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesLibrary;
using MoviesWebApp.ViewModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace MoviesWebApp.Controllers
{
    // TODO: allow anonymous access
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private MovieIdentityService _identityService;

        public AccountController(MovieIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // TODO: validate password
            // create list of claims w/ sub claim from username
            // call into MovieIdentityService to get app specific claims and merge into claims list
            // create claims identity and claims principal
            // call signin to issue cookie
            // redirect to return url, or back to home page
            if(model.Username == model.Password)
            {
                var claims = new List<Claim>
                {
                    new Claim("sub", model.Username)
                };
                claims.AddRange(_identityService.GetClaimsForUser(model.Username));

                var ci = new ClaimsIdentity(claims, "password", "name", "role");
                var claimsPrincipal = new ClaimsPrincipal(ci);

                await HttpContext.SignInAsync(claimsPrincipal);

                if(model.ReturnUrl != null)
                {
                    return LocalRedirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // TODO: remove authentication cookie by calling signout
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
