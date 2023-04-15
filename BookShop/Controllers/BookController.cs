using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class BookController : Controller
    {
        private readonly BookDbContext _db;
        public BookController(BookDbContext ob) { _db = ob; }
        public IActionResult Show()
        {
            ViewBag.Books = _db.Books;
            return View();
        }

        public IActionResult Create()
        {
            ViewBag.authors=_db.Authors.Select(x => new{ x.Name,x.Id} ).ToList();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book, IFormFile ImageFile)
        {
            string originalpath = "";
            if (ImageFile != null && ImageFile.Length > 0)
            {
                originalpath = DateTime.Now.Ticks + ImageFile.FileName;
                book.BookImageUrl = "/images/" + originalpath;
            }
            if (ModelState.IsValid)
            {

                try
                {
                    var filePath = Path.Combine("wwwroot/images", originalpath);
                    var stream = new FileStream(filePath, FileMode.Create);
                    await ImageFile.CopyToAsync(stream);
                    _db.Add(book);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["message"] = "501 Internal Server Error";
                    return RedirectToAction("Index", "Error");
                }

                return RedirectToAction("Show");
            }
            return View();
        }

    }
}
