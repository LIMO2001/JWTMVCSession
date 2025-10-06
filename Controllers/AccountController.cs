using JwtSessionMvc.Models;
using JwtSessionMvc.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtSessionMvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private const string SessionKeyToken = "JWToken";

        public AccountController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // -------- REGISTER --------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "Both fields are required!";
                return View();
            }

            var existingUser = await _userService.GetUserByEmailAsync(email);
            if (existingUser != null)
            {
                TempData["Error"] = "Email already exists!";
                return View();
            }

            var newUser = new User
            {
                Email = email,
                PasswordHash = password
            };

            await _userService.AddUserAsync(newUser);

            TempData["Success"] = "User created successfully! You can now login.";
            return RedirectToAction("Login");
        }

        // -------- LOGIN --------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.ValidateUserAsync(email, password);
            if (user == null)
            {
                TempData["Error"] = "Invalid credentials!";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var token = _jwtService.GenerateToken(user.Email, claims);
            HttpContext.Session.SetString(SessionKeyToken, token);

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            TempData["Success"] = "Login successful! Welcome back.";
            return RedirectToAction("Index", "Dashboard");
        }

        // -------- LOGOUT --------
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove(SessionKeyToken);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "You have logged out successfully.";
            return RedirectToAction("Login");
        }

        //SHOW JWT TOKEN (DEBUG ONLY)
        [HttpGet]
        public IActionResult ShowToken()
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            var token = HttpContext.Session.GetString(SessionKeyToken);
            return Json(new { token });
        }
    }
}
