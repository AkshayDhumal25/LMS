//using LMS.Data;
//using LMS.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace LMS.Controllers
//{
//    public class BorrowController : Controller
//    {
//        private readonly AppDbContext _context;

//        public BorrowController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // List available books
//        public IActionResult AvailableBooks()
//        {
//            var books = _context.Books.ToList();
//            return View(books);
//        }

//        // Borrow a book
//        public IActionResult BorrowBook(int id)
//        {
//            var book = _context.Books.Find(id);
//            if (book == null) return NotFound();

//            var userId = HttpContext.Session.GetInt32("UserId");
//            if (userId == null) return RedirectToAction("Login", "User");

//            var record = new BorrowRecord
//            {
//                BookId = id,
//                UserId = userId.Value,
//                BorrowedAt = DateTime.Now,
//                IsReturned = false
//            };

//            _context.BorrowRecords.Add(record);
//            _context.SaveChanges();

//            return RedirectToAction("MyBorrowedBooks");
//        }

//        // List user’s borrowed books
//        public IActionResult MyBorrowedBooks()
//        {
//            var userId = HttpContext.Session.GetInt32("UserId");
//            if (userId == null) return RedirectToAction("Login", "User");

//            var records = _context.BorrowRecords
//                .Include(b => b.Book)
//                .Where(r => r.UserId == userId)
//                .ToList();

//            return View(records);
//        }

//        // Return a book
//        public IActionResult ReturnBook(int id)
//        {
//            var record = _context.BorrowRecords.Find(id);
//            if (record == null) return NotFound();

//            record.IsReturned = true;
//            record.ReturnedAt = DateTime.Now;

//            _context.SaveChanges();
//            return RedirectToAction("MyBorrowedBooks");
//        }
//    }

//}


using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMS.Controllers
{
    public class BorrowController : Controller
    {
        private readonly AppDbContext _context;

        public BorrowController(AppDbContext context)
        {
            _context = context;
        }

        // List available books

        //public IActionResult AvailableBooks(string searchQuery)
        //{
        //    Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
        //    Response.Headers["Pragma"] = "no-cache";
        //    Response.Headers["Expires"] = "0";
        //    var books = _context.Books.AsQueryable();



        //    if (!string.IsNullOrEmpty(searchQuery))
        //    {
        //        books = books.Where(b =>
        //            b.Title.Contains(searchQuery) ||
        //            b.Author.Contains(searchQuery) ||
        //            b.Category.Contains(searchQuery)
        //        );
        //    }

        //    return View(books.ToList());
        //}


        public IActionResult AvailableBooks(string searchQuery, int pageNumber = 1, int pageSize = 5)
        {
            // Prevent caching
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            var books = _context.Books.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchQuery))
            {
                books = books.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Category.Contains(searchQuery)
                );
            }

            // Get total count after filtering
            var totalBooks = books.Count();

            // Apply pagination
            books = books
                .OrderBy(b => b.Title)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // Send pagination data to view
            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalBooks = totalBooks;
            ViewBag.SearchQuery = searchQuery;

            return View(books.ToList());
        }


        // Borrow a book
        public IActionResult BorrowBook(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null || book.TotalCopies <= 0)
                return NotFound();

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var record = new BorrowRecord
            {
                BookId = id,
                UserId = userId.Value,
                BorrowedAt = DateTime.Now,
                IsReturned = false
            };

            book.TotalCopies -= 1;
            _context.BorrowRecords.Add(record);
            _context.SaveChanges();

            return RedirectToAction("MyBorrowedBooks");
        }

        // List user’s borrowed books
        public IActionResult MyBorrowedBooks()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var records = _context.BorrowRecords
                .Include(b => b.Book)
                .Where(r => r.UserId == userId)
                .ToList();

            return View(records);
        }

        // Return a book
        //public IActionResult ReturnBook(int id)
        //{
        //    var record = _context.BorrowRecords
        //        .Include(r => r.Book)
        //        .FirstOrDefault(r => r.Id == id);

        //    if (record == null || record.IsReturned)
        //        return NotFound();

        //    record.IsReturned = true;
        //    record.ReturnedAt = DateTime.Now;
        //    record.Book.TotalCopies += 1;

        //    _context.SaveChanges();
        //    return RedirectToAction("MyBorrowedBooks");
        //}
        public IActionResult ReturnBook(int id)
        {
            var record = _context.BorrowRecords.Find(id);
            if (record == null) return NotFound();

            // Mark the book as returned
            record.IsReturned = true;
            record.ReturnedAt = DateTime.Now;

            // Also increment the book copies back
            var book = _context.Books.Find(record.BookId);
            if (book != null)
            {
                book.TotalCopies += 1;  // Return the borrowed copy to the available stock
                _context.SaveChanges();
            }

            // Save the changes to the borrow record
            _context.SaveChanges();
            return RedirectToAction("MyBorrowedBooks");
        }


        public IActionResult SearchBooks(string searchQuery)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                books = books.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Category.Contains(searchQuery)
                );
            }

            return View(books.ToList());
        }


        


    }
}

