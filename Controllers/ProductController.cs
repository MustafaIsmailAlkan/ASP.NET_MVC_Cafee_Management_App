using Cafee_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;



namespace Cafee_Prototype.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        readonly CafeeDbContext _cafeeDbContext;
        public ProductController(CafeeDbContext cafeeDbContext)
        {
            _cafeeDbContext = cafeeDbContext;
        }

        public async Task<IActionResult> ListProducts()
        {
            var list = await _cafeeDbContext.ProductCategories.Include(c => c.Products).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> AddProduct()
        {
            ViewBag.ProductCategories = new SelectList(
                await _cafeeDbContext.ProductCategories.ToListAsync(),
                "ProductCategoryId",
                "CategoryName"
            );
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product, IFormFile imageFile)
        {
            if(imageFile == null)
            {
                Console.WriteLine("IMAAGE FILE IS NULL");
                return BadRequest("IMAAGE FILE IS NULL");
            }
            
            if (product == null || !ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return NotFound();
            }
            //Category Ekleme Kısmı
            ModelState.Clear();
            ViewBag.ProductCategories = new SelectList(
                    _cafeeDbContext.ProductCategories.ToList(),
                    "ProductCategoryId",
                    "CategoryName"
                );

            var extension = Path.GetExtension(imageFile.FileName);
            var randomFileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + Guid.NewGuid().ToString() + extension;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

            using(var stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            product.ProductImage = randomFileName;

            _cafeeDbContext.Add(product);
            await _cafeeDbContext.SaveChangesAsync();
            return RedirectToAction("ListProducts");
        }

        public async Task<IActionResult> EditProduct(int id)
        {
            ModelState.Clear();
            ViewBag.ProductCategories = new SelectList(
                    _cafeeDbContext.ProductCategories.ToList(),
                    "ProductCategoryId",
                    "CategoryName"
                );
            return View(await _cafeeDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id));
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            if (product == null || !ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return NotFound();
            }
            

            if(imageFile != null)
            {
                //Getting new image
                var extension = Path.GetExtension(imageFile.FileName);
                var randomFileName = Path.GetFileNameWithoutExtension(imageFile.FileName) + Guid.NewGuid().ToString() + extension;
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", randomFileName);

                using(var stream = new FileStream(path, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                
                //Deleting old image
                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", product.ProductImage = null!);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                product.ProductImage = randomFileName;
            }
            

             _cafeeDbContext.Update(product);
            await _cafeeDbContext.SaveChangesAsync();

            return RedirectToAction("ListProducts", "Product");
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _cafeeDbContext.Products.FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return BadRequest("Bu item bulunamadı id: " + id);
            }

            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", product.ProductImage);
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _cafeeDbContext.Products.Remove(product);
            await _cafeeDbContext.SaveChangesAsync();
            return RedirectToAction("ListProducts", "Product");
        }


    }
}