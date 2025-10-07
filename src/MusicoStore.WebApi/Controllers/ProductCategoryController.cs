using Microsoft.AspNetCore.Mvc;
using MusicoStore.Application.Abstractions.Interfaces;
using MusicoStore.Application.Interfaces.IRepositories;
using MusicoStore.Domain.Entities;

namespace MusicoStore.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryRepository _categoryRepository;

        public ProductCategoryController(IProductCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductCategory>> GetAll() => await _categoryRepository.GetAllAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return NotFound();
            return category;
        }

        [HttpPost]
        public async Task<ActionResult<ProductCategory>> Create(ProductCategory category)
        {
            var created = await _categoryRepository.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ProductCategory>> Update(int id, ProductCategory category)
        {
            if (id != category.Id) return BadRequest();
            return await _categoryRepository.UpdateAsync(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
