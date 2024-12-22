using Ecommerce.Model;
using Ecommerce.ProductService.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.ProductService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ProductDbContextcs dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
    {
        return await dbContext.Products.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ProductModel> GetProduct(int id)
    {
        return await dbContext.Products.FindAsync(id);
    }
}
