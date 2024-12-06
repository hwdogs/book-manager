using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;
using book_dotnet.Data;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("book")]
    public class BookController : ControllerBase
    {
        private readonly BookDbContext _context;

        public BookController(BookDbContext context)
        {
            _context = context;
        }

        [HttpGet("findAll/{page}/{size}")]
        public async Task<ActionResult<object>> FindAll(int page, int size)
        {
            var totalElements = await _context.Books.CountAsync();
            var books = await _context.Books
                .Skip(page * size)
                .Take(size)
                .ToListAsync();

            // 构造与Spring Boot相同的返回格式
            var result = new
            {
                content = books,
                pageable = new
                {
                    sort = new
                    {
                        sorted = false,
                        unsorted = true,
                        empty = true
                    },
                    pageSize = size,
                    pageNumber = page,
                    offset = page * size,
                    unpaged = false
                },
                totalElements = totalElements,
                totalPages = (int)Math.Ceiling(totalElements / (double)size),
                last = (page + 1) * size >= totalElements,
                size = size,
                number = page,
                sort = new
                {
                    sorted = false,
                    unsorted = true,
                    empty = true
                },
                numberOfElements = books.Count,
                first = page == 0,
                empty = !books.Any()
            };

            return result;
        }

        [HttpGet("findById/{id}")]
        public async Task<ActionResult<Book>> FindById(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }

        [HttpPost("save")]
        public async Task<ActionResult<string>> Save(Book book)
        {
            _context.Books.Add(book);
            var result = await _context.SaveChangesAsync();

            return result > 0 ? "success" : "error";
        }

        [HttpPut("update")]
        public async Task<ActionResult<string>> Update([FromBody] Book book)
        {
            try
            {
                // 检查书籍是否存在
                var existingBook = await _context.Books.FindAsync(book.Id);
                if (existingBook == null)
                {
                    return "error";
                }

                // 更新书籍信息
                existingBook.Name = book.Name;
                existingBook.Author = book.Author;
                existingBook.Publish = book.Publish;

                // 保存更改
                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> Search([FromQuery] string keyword)
        {
            try
            {
                var query = _context.Books.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    keyword = keyword.ToLower();
                    query = query.Where(b =>
                        EF.Functions.Like(b.Name.ToLower(), $"%{keyword}%") ||
                        EF.Functions.Like(b.Author.ToLower(), $"%{keyword}%") ||
                        EF.Functions.Like(b.Publish.ToLower(), $"%{keyword}%") ||
                        (b.Id.ToString() == keyword)
                    );
                }

                var books = await query.ToListAsync();
                return Ok(books);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}