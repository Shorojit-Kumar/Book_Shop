using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            if (book.AuthorId != null)
            {
                var x = _db.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
                book.AuthorName = x.Name;
            }
            if (ModelState.IsValid)
            {

                try
                {
                    var filePath = Path.Combine("wwwroot/images", originalpath);
                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                   
                    book.CreatedAt = DateTime.Now;
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



        public IActionResult BookDetails(int Id)
        {
            var book = _db.Books.FirstOrDefault(a => a.Id == Id);
            if (book == null)
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.book = book;
            return View();
        }


        public IActionResult Delete(int Id)
        {
            var book = _db.Books.FirstOrDefault(a => a.Id == Id);

            if (book != null)
            {
                try
                {
                    var dir = Directory.GetCurrentDirectory();
                  
                    // Delete book image
                    string fullPath = Path.Combine(dir, "wwwroot/" + book.BookImageUrl);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
            
                    _db.Remove(book);
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    TempData["message"] = "501 Internal Server Error";
                    return RedirectToAction("Index", "Error");
                }
            }

            return RedirectToAction("Show");
        }


        public IActionResult Edit(int Id)
        {
            var book = _db.Books.SingleOrDefault(a => a.Id == Id);
            ViewBag.authors = _db.Authors.Select(x => new { x.Name, x.Id }).ToList();
            return View(book);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Book book, IFormFile ImageFile)
        {
            try
            {
                var obj = _db.Books.SingleOrDefault(a => a.Id == book.Id);
                if (obj != null)
                {
                    book.Title = (book.Title!=null)? book.Title:obj.Title;
                    book.Description = (book.Description != null)? book.Description : obj.Description;
                    book.PublishedAt = (book.PublishedAt != null)? book.PublishedAt : obj.PublishedAt;
                    book.AuthorId = (book.AuthorId != null)? book.AuthorId : obj.AuthorId;
                    book.AuthorName = (book.AuthorName != null)? book.AuthorName : obj.AuthorName;
                    book.BookImageUrl = (book.BookImageUrl != null)? book.BookImageUrl : obj.BookImageUrl;
                    book.CreatedAt = (book.CreatedAt != null)? book.CreatedAt : obj.CreatedAt;
                }
                if (ImageFile == null)
                {
                    if (obj != null)
                    {
                        book.BookImageUrl = obj.BookImageUrl;
                    }
                }
                else if (obj.BookImageUrl != "/images/DefaultBook.jpg")
                {
                    //Delete previous Image
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", book.BookImageUrl);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    //Write new image 
                    var originalpath = DateTime.Now.Ticks + ImageFile.FileName;
                    book.BookImageUrl = "/images/" + originalpath;
                    var filePath = Path.Combine("wwwroot", "images", originalpath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);

                    }
                }
                _db.Books.Update(book);
                _db.SaveChanges();
                return RedirectToAction("Show");
            }
            catch (FileNotFoundException)
            {
                TempData["message"] = "Image file not found.";
                return RedirectToAction("Index", "Error");
            }
            catch (DirectoryNotFoundException)
            {
                TempData["message"] = "Image directory not found.";
                return RedirectToAction("Index", "Error");
            }
            catch (Exception ex)
            {
                TempData["message"] = "Internal server error.";
                return RedirectToAction("Index", "Error");
            }
        }
    }
}
