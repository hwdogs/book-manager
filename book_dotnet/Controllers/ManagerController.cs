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
        public async Task<ActionResult<object>> Login([FromBody] Manager manager)
        {
            Console.WriteLine($"Login attempt - Name: {manager.Name}");

            var result = await _context.Managers
                .FirstOrDefaultAsync(m => m.Name == manager.Name && m.Password == manager.Password);

            if (result != null)
            {
                Console.WriteLine($"Login successful - Returning name: {result.Name}");
                return new
                {
                    status = "success",
                    name = result.Name  // 返回实际的用户名
                };
            }
            Console.WriteLine("Login failed");
            return new { status = "error" };
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

        [HttpPut("updateProfile")]
        public async Task<ActionResult<string>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            try
            {
                var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Name == request.Username);
                if (manager == null)
                {
                    return "notfound";
                }

                // 验证旧密码
                if (manager.Password != request.OldPassword)
                {
                    return "wrongpassword";
                }

                // 更新密码
                manager.Password = request.NewPassword;
                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        public class UpdateProfileRequest
        {
            public string Username { get; set; }
            public string OldPassword { get; set; }
            public string NewPassword { get; set; }
        }

        private async Task<bool> ManagerExists(int id)
        {
            return await _context.Managers.AnyAsync(e => e.Id == id);
        }
    }
}