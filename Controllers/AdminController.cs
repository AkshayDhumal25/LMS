//using System;
//using LMS.Data;
//using LMS.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace LMS.Controllers
//{
//    public class AdminController : Controller
//    {
//        private readonly AppDbContext _context;

//        public AdminController(AppDbContext context)
//        {
//            _context = context;
//        }

//        private bool IsAdmin()
//        {
//            var isAdmin = HttpContext.Session.GetString("IsAdmin");
//            return isAdmin != null && isAdmin == "True";
//        }

//        // GET: /Admin/UserList
//        public IActionResult UserList()
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Account");

//            var users = _context.ApplicationUsers.ToList();
//            return View(users);
//        }

//        // GET: /Admin/AddUser
//        public IActionResult AddUser()
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Account");

//            return View();
//        }

//        // POST: /Admin/AddUser
//        [HttpPost]
//        public IActionResult AddUser(ApplicationUser user)
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Account");

//            if (ModelState.IsValid)
//            {
//                _context.ApplicationUsers.Add(user);
//                _context.SaveChanges();
//                return RedirectToAction("UserList");
//            }
//            return View(user);
//        }

//        // GET: /Admin/EditUser/{id}
//        public IActionResult EditUser(int id)
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Account");

//            var user = _context.ApplicationUsers.Find(id);
//            if (user == null) return NotFound();

//            return View(user);
//        }

//        // POST: /Admin/EditUser
//        [HttpPost]
//        public IActionResult EditUser(ApplicationUser updatedUser)
//        {
//            if (!IsAdmin()) return RedirectToAction("Login", "Account");

//            var user = _context.ApplicationUsers.Find(updatedUser.Id);
//            if (user != null)
//            {
//                user.Name = updatedUser.Name;
//                user.Username = updatedUser.Username;
//                user.Password = updatedUser.Password;
//                user.Email = updatedUser.Email;
//                user.ContactNumber = updatedUser.ContactNumber;
//                user.IsAdmin = updatedUser.IsAdmin;

//                _context.SaveChanges();
//                return RedirectToAction("UserList");
//            }
//            return View(updatedUser);
//        }
//    }
//}


using System.Linq;
using System.Text;
using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;



namespace LMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Check if the current user is an admin
        private bool IsAdmin()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            return isAdmin != null && bool.TryParse(isAdmin, out var result) && result;
        }

        // GET: /Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Admin/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.ApplicationUsers
                .FirstOrDefault(u => u.Username == username && u.Password == password && u.IsAdmin);

            if (admin != null)
            {
                HttpContext.Session.SetString("IsAdmin", "True");
                HttpContext.Session.SetString("AdminName", admin.Name);
                return RedirectToAction("Dashboard"); // ✅ Correct redirect
            }
            ViewBag.Error = "Invalid admin credentials.";
            return View();
        }

        // GET: /Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("AdminName");
            return RedirectToAction("Login");
        }

        // GET: /Admin/UserList
        public IActionResult UserList()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var users = _context.ApplicationUsers.ToList();
            return View(users);
        }

        // GET: /Admin/AddUser
        public IActionResult AddUser()
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            return View();
        }

        // POST: /Admin/AddUser
        [HttpPost]
        public IActionResult AddUser(ApplicationUser user)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            if (ModelState.IsValid)
            {
                user.IsAdmin = false;
                _context.ApplicationUsers.Add(user);
                _context.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        // GET: /Admin/EditUser/{id}
        public IActionResult EditUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var user = _context.ApplicationUsers.Find(id);
            if (user == null) return RedirectToAction("UserList");

            return View(user);
        }

        // POST: /Admin/EditUser
        [HttpPost]
        public IActionResult EditUser(ApplicationUser updatedUser)
        {
            if (!IsAdmin()) return RedirectToAction("Login");

            var user = _context.ApplicationUsers.Find(updatedUser.Id);
            if (user != null)
            {
                user.Name = updatedUser.Name;
                user.Username = updatedUser.Username;
                user.Password = updatedUser.Password;
                user.Email = updatedUser.Email;
                user.ContactNumber = updatedUser.ContactNumber;
                user.IsAdmin = updatedUser.IsAdmin;

                _context.SaveChanges();
                return RedirectToAction("UserList");
            }
            return View(updatedUser);
        }


        public IActionResult DeleteUser(int id)
        {
            var user = _context.ApplicationUsers.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.ApplicationUsers.Remove(user);
            _context.SaveChanges();

            return RedirectToAction("UserList");
        }




        public IActionResult Dashboard()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }


        // AdminController.cs

        public IActionResult BorrowHistory()
        {
            // Get all borrow records with user and book details
            var borrowHistory = _context.BorrowRecords
                .Include(b => b.User)  // Include user details
                .Include(b => b.Book)  // Include book details
                .ToList();

            return View(borrowHistory);  // Return the data to the view
        }


        // Export to CSV
        public IActionResult ExportBooksToCsv()
        {
            var books = _context.Books.ToList();

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("Id,Title,Author,ISBN,Category,TotalCopies");

            foreach (var book in books)
            {
                csvBuilder.AppendLine($"{book.Id},{book.Title},{book.Author},{book.ISBN},{book.Category},{book.TotalCopies}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csvBuilder.ToString());

            return File(buffer, "text/csv", "books.csv");
        }

        


    }
}
