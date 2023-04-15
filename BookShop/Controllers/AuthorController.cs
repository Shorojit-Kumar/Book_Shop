using BookShop.Data;
using BookShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookShop.Controllers
{
    public class AuthorController : Controller
    {
        private readonly BookDbContext _db;
        public AuthorController(BookDbContext ob) { _db = ob; }
        public IActionResult Show()
        {
            ViewBag.Authors=_db.Authors;
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> Create(Author author,IFormFile ImageFile)
        {
            string originalpath = "";
            if(ImageFile != null && ImageFile.Length > 0)
            {
                originalpath = DateTime.Now.Ticks + ImageFile.FileName;
                author.AuthorImageURL = "/images/" + originalpath;
            }
            if(ModelState.IsValid)
            {

                try
                {
                    var filePath = Path.Combine("wwwroot/images", originalpath);
                    var stream = new FileStream(filePath, FileMode.Create);
                    await ImageFile.CopyToAsync(stream);
                    _db.Add(author);
                    _db.SaveChanges();
                }
                catch(Exception ex)
                {
                    ViewBag.message = "501 Internal Server Error";
                    return RedirectToAction("Index","Error");
                }

                return RedirectToAction("Show");
            }
            return View();
        }


        public IActionResult AuthorDetails(int Id)
        {
            var author = _db.Authors.Include(a => a.BookList).FirstOrDefault(a => a.Id == Id);
            if(author == null)
            {
                return RedirectToAction("Index", "Error");
            }
            ViewBag.author = author;    
            return View(author);
        }

        public IActionResult Delete(int Id)
        {
            var author = _db.Authors.Include(a => a.BookList).SingleOrDefault(a => a.Id == Id);

            if (author != null)
            {
                _db.RemoveRange(author.BookList);
                _db.Remove(author);
                _db.SaveChanges();
            }
            return RedirectToAction("Show");
        }

    }
}
