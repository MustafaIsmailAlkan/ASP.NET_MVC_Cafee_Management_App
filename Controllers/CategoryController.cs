using Cafee_Prototype.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafee_Prototype.Controllers;

[Authorize]
public class CategoryController : Controller
{
    private readonly CafeeDbContext _cafeeDbContext;
    public CategoryController(CafeeDbContext cafeeDbContext)
    {
        _cafeeDbContext = cafeeDbContext;
    }

    public async Task<IActionResult> ListCategories()
    {
        var categories = await _cafeeDbContext.ProductCategories.ToListAsync();
        return View(categories);
    }

    public IActionResult AddCategory()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCategory(ProductCategory category)
    {
        if(category == null)
        {
            return BadRequest("category is NULL");
        }

        await _cafeeDbContext.ProductCategories.AddAsync(category);
        await _cafeeDbContext.SaveChangesAsync();
        return RedirectToAction("ListCategories");
    }

    public async Task<IActionResult> EditCategory(int id)
    {
        if(id == 0)
        {
            return BadRequest("id is 0");
        }

        var category = await _cafeeDbContext.ProductCategories.FirstOrDefaultAsync(c => c.ProductCategoryId == id);
        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> EditCategory(ProductCategory category)
    {
        if(category == null)
        {
            return BadRequest("category is NULL");
        }

        _cafeeDbContext.ProductCategories.Update(category);
        await _cafeeDbContext.SaveChangesAsync();
        return RedirectToAction("ListCategories");
    }

    public async Task<IActionResult> DeleteCategory(int id)
    {
        if(id == 0)
        {
            return BadRequest("id is 0");
        }

        var category = await _cafeeDbContext.ProductCategories.FirstOrDefaultAsync(c => c.ProductCategoryId == id);
        if(category == null)
        {
            return BadRequest("category is NULL");
        }
        _cafeeDbContext.ProductCategories.Remove(category);
        await _cafeeDbContext.SaveChangesAsync();
        
        return RedirectToAction("ListCategories");
    }
}