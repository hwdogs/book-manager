using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;
using book_dotnet.Data;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("manager")]
    public class ManagerController : ControllerBase
    {
        private readonly BookDbContext _context;

        public ManagerController(BookDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] Manager loginManager)
        {
            var manager = await _context.Managers
                .FirstOrDefaultAsync(m => m.Name == loginManager.Name && m.Password == loginManager.Password);

            if (manager != null)
            {
                return "success";
            }
            return "error";
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody] Manager newManager)
        {
            // 检查用户名是否已存在
            var exists = await _context.Managers.AnyAsync(m => m.Name == newManager.Name);
            if (exists)
            {
                return "exists";
            }

            _context.Managers.Add(newManager);
            var result = await _context.SaveChangesAsync();

            return result > 0 ? "success" : "error";
        }

        [HttpGet("findAll")]
        public async Task<ActionResult<IEnumerable<Manager>>> FindAll()
        {
            return await _context.Managers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Manager>> GetById(int id)
        {
            var manager = await _context.Managers.FindAsync(id);

            if (manager == null)
            {
                return NotFound();
            }

            return manager;
        }

        [HttpPost]
        public async Task<ActionResult<Manager>> Create(Manager manager)
        {
            _context.Managers.Add(manager);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = manager.Id }, manager);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Manager manager)
        {
            if (id != manager.Id)
            {
                return BadRequest();
            }

            _context.Entry(manager).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ManagerExists(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager == null)
            {
                return NotFound();
            }

            _context.Managers.Remove(manager);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<bool> ManagerExists(int id)
        {
            return await _context.Managers.AnyAsync(e => e.Id == id);
        }
    }
}