using System;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                user.IsAdmin = false; // Regular user by default
                _context.ApplicationUsers.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _context.ApplicationUsers.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                if (user.IsAdmin)
                    return RedirectToAction("UserList", "Admin");
                else
                    return RedirectToAction("Profile");
            }

            ViewBag.Error = "Invalid credentials.";
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/Profile
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.ApplicationUsers.Find(userId);
            return View(user);
        }

        // GET: /Account/EditProfile
        public IActionResult EditProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.ApplicationUsers.Find(userId);
            return View(user);
        }

        // POST: /Account/EditProfile
        [HttpPost]
        public IActionResult EditProfile(ApplicationUser updatedUser)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login");

            var user = _context.ApplicationUsers.Find(userId);

            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Email = updatedUser.Email;
                user.ContactNumber = updatedUser.ContactNumber;
                user.Password = updatedUser.Password;

                _context.SaveChanges();
                return RedirectToAction("Profile");
            }

            return View(updatedUser);
        }
    }
}
