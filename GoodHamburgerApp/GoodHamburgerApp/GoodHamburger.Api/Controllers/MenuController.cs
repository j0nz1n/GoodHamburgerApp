
using GoodHamburger.Api.Data;
using GoodHamburger.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly AppDbContext _context;
        public MenuController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenu()
        {
            return await _context.MenuItems.ToListAsync();
        }
    }
}
