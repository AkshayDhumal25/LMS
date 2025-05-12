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
        public IActionResult Index()
        {
            var books = _context.Books.ToList();
            return View(books);
        }

        // GET: /Book/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Book/Add
        [HttpPost]
        public IActionResult Add(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Add(book);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // GET: /Book/Edit/{id}
        public IActionResult Edit(int id)
        {
            var book = _context.Books.Find(id);
            if (book == null) return NotFound();
            return View(book);
        }

        // POST: /Book/Edit
        [HttpPost]
        public IActionResult Edit(Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Books.Update(book);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
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
    }
}
