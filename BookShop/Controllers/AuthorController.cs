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
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                        
                    author.CreatedAt = DateTime.Now;
                    _db.Add(author);
                    _db.SaveChanges();
                }
                catch(Exception ex)
                {
                    TempData["message"] = "501 Internal Server Error";
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
                try
                {
                    var dir = Directory.GetCurrentDirectory();
                    foreach (var book in author.BookList)
                    {
                        // Delete book image
                        var fullPath1 = Path.Combine(dir, "wwwroot/" + book.BookImageUrl);
                        if (System.IO.File.Exists(fullPath1))
                        {
                            System.IO.File.Delete(fullPath1);
                        }
                    }

                    // Delete author image
                    string fullPath = Path.Combine(dir, "wwwroot/" + author.AuthorImageURL);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    _db.RemoveRange(author.BookList);
                    _db.Remove(author);
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
            var author = _db.Authors.SingleOrDefault(a => a.Id == Id);
            return View(author);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Author author, IFormFile ImageFile)
        {
            try
            {
                var obj = _db.Authors.SingleOrDefault(a => a.Id == author.Id);
                if (ImageFile == null)
                {
                    if (obj != null)
                    {
                        author.AuthorImageURL = obj.AuthorImageURL;
                    }
                }
                else if (obj.AuthorImageURL != "/images/Default.jpg")
                {
                    //Delete previous Image
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", author.AuthorImageURL);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }

                    //Write new image 
                    var originalpath = DateTime.Now.Ticks + ImageFile.FileName;
                    author.AuthorImageURL = "/images/" + originalpath;
                    var filePath = Path.Combine("wwwroot", "images", originalpath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                        
                    }
                }
                
                _db.Authors.Update(author);
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
