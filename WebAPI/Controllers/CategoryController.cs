using Business.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entities.Concrete;
using Entities.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : ControllerBase
    {
        ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("addcategory")]
        public IActionResult AddCategory(CategoryDto categoryDto)
        {
            Category category = new Category();
            category.Name = categoryDto.Name;
            _categoryService.Add(category);

            return Ok(category);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            var category = _categoryService.Get(c=>c.Id == id);
            category.Name = categoryDto.Name;
            _categoryService.Update(category);
            return Ok(category);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                var category = _categoryService.Get(c => c.Id == id);
                if (category == null)
                {
                    return NotFound();
                }
                _categoryService.Delete(category);
                return Ok("Başarıyla silindi.");
            }
            catch (Exception)
            {

                throw;
            }
              
        }
    }
}
