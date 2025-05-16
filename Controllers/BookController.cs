//using Microsoft.AspNetCore.Mvc;

//namespace LMS.Controllers
//{
//    public class BookController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}


using LMS.Data;
using LMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LMS.Controllers
{
    public class BookController : Controller
    {
        private readonly AppDbContext _context;

        public BookController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Book/Index
        //public IActionResult Index()
        //{
        //    var books = _context.Books.ToList();
        //    return View(books);
        //}
        public IActionResult Index(string searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                booksQuery = booksQuery.Where(b =>
                    b.Title.Contains(searchQuery) ||
                    b.Author.Contains(searchQuery) ||
                    b.Category.Contains(searchQuery)
                );
            }

            var totalBooks = booksQuery.Count();

            var books = booksQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalBooks = totalBooks;
            ViewBag.SearchQuery = searchQuery;

            return View(books);
        }


        //GET: /Book/Add
        //public IActionResult Add()
        //{
        //    return View();
        //}

        ////POST: /Book/Add
        //[HttpPost]
        // public IActionResult Add(Book book)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Books.Add(book);
        //        _context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(book);
        //}

        // BookController.cs

        // GET: /Book/Add
        public IActionResult Add()
        {
            ViewBag.Rows = _context.Rows
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();

            return View();
        }

        // POST: /Book/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Book book)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Re-populate dropdown on validation error
            //    ViewBag.Rows = _context.Rows
            //        .Select(r => new SelectListItem
            //        {
            //            Value = r.Id.ToString(),
            //            Text = r.Name
            //        }).ToList();

            //    return View(book);
            //}

            // ✅ Raw SQL Insert

            _context.Database.ExecuteSqlRaw(
                "INSERT INTO Books (Title, Author, ISBN, Category, TotalCopies, RowId) VALUES ({0}, {1}, {2}, {3}, {4}, {5})",
                book.Title, book.Author, book.ISBN, book.Category, book.TotalCopies, book.RowId
            );

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            ViewBag.Rows = _context.Rows
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.Name
                }).ToList();


            return View(book);
        }

        // POST: /Book/Edit
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            
            
                _context.Books.Update(book);
                _context.SaveChanges();
               
            
            return RedirectToAction("Index");
        }

        // GET: /Book/Delete/{id}
        public IActionResult Delete(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();

            _context.Books.Remove(book);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            ViewBag.Sections = _context.Sections.ToList();
            return View();
        }

        public JsonResult GetShelvesBySection(int id)
        {
            var shelves = _context.Shelves
                .Where(s => s.SectionId == id)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Json(shelves);
        }

        public JsonResult GetRowsByShelf(int id)
        {
            var rows = _context.Rows
                .Where(r => r.ShelfId == id)
                .Select(r => new { r.Id, r.Name })
                .ToList();

            return Json(rows);
        }

    }
}


