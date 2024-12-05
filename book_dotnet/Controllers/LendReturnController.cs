using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;
using book_dotnet.Data;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("lendReturn")]
    public class LendReturnController : ControllerBase
    {
        private readonly BookDbContext _context;

        public LendReturnController(BookDbContext context)
        {
            _context = context;
        }

        [HttpGet("findAll/{page}/{size}")]
        public async Task<ActionResult<object>> FindAll(int page, int size)
        {
            try
            {
                var totalElements = await _context.LendReturns.CountAsync();
                var records = await _context.LendReturns
                    .Include(lr => lr.Reader)
                    .Include(lr => lr.Book)
                    .OrderBy(lr => lr.Id)
                    .Skip(page * size)
                    .Take(size)
                    .Select(lr => new
                    {
                        id = lr.Id,
                        readerId = lr.ReaderId,
                        readerName = lr.Reader.Name,
                        bookId = lr.BookId,
                        bookName = lr.Book.Name,
                        lendDate = lr.LendDate,
                        returnDate = lr.ReturnDate
                    })
                    .ToListAsync();

                Console.WriteLine($"Found {records.Count} records");
                foreach (var record in records)
                {
                    Console.WriteLine($"Record: {record.id}, Reader: {record.readerName}, Book: {record.bookName}");
                }

                var result = new
                {
                    content = records,
                    pageable = new
                    {
                        sort = new { sorted = false, unsorted = true, empty = true },
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
                    sort = new { sorted = false, unsorted = true, empty = true },
                    numberOfElements = records.Count,
                    first = page == 0,
                    empty = !records.Any()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindAll: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("lend")]
        public async Task<ActionResult<string>> LendBook([FromBody] LendRequest request)
        {
            try
            {
                // 添加日志
                Console.WriteLine($"Received lend request - ReaderId: {request.ReaderId}, BookId: {request.BookId}");

                // 检查读者和图书是否存在
                var reader = await _context.Readers.FindAsync(request.ReaderId);
                var book = await _context.Books.FindAsync(request.BookId);

                if (reader == null)
                {
                    Console.WriteLine("Reader not found");
                    return "error";
                }

                if (book == null)
                {
                    Console.WriteLine("Book not found");
                    return "error";
                }

                // 检查图书是否已被借出
                var existingLend = await _context.LendReturns
                    .Where(lr => lr.BookId == request.BookId && lr.ReturnDate == null)
                    .FirstOrDefaultAsync();

                if (existingLend != null)
                {
                    Console.WriteLine("Book already borrowed");
                    return "borrowed";
                }

                // 创建新的借阅记录
                var lendReturn = new LendReturn
                {
                    ReaderId = request.ReaderId,
                    BookId = request.BookId,
                    LendDate = DateTime.Now,
                    ReturnDate = null
                };

                _context.LendReturns.Add(lendReturn);
                await _context.SaveChangesAsync();

                Console.WriteLine("Lend successful");
                return "success";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LendBook: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return "error";
            }
        }

        [HttpPost("return/{id}")]
        public async Task<ActionResult<string>> ReturnBook(int id)
        {
            try
            {
                var record = await _context.LendReturns.FindAsync(id);
                if (record == null || record.ReturnDate != null)
                {
                    return "error";
                }

                record.ReturnDate = DateTime.Now;
                await _context.SaveChangesAsync();

                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        [HttpGet("findByReader/{readerId}")]
        public async Task<ActionResult<IEnumerable<object>>> FindByReader(int readerId)
        {
            try
            {
                var records = await _context.LendReturns
                    .Include(lr => lr.Book)
                    .Where(lr => lr.ReaderId == readerId)
                    .Select(lr => new
                    {
                        id = lr.Id,
                        bookId = lr.BookId,
                        bookName = lr.Book.Name,
                        lendDate = lr.LendDate,
                        returnDate = lr.ReturnDate
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("findByBook/{bookId}")]
        public async Task<ActionResult<IEnumerable<object>>> FindByBook(int bookId)
        {
            try
            {
                var records = await _context.LendReturns
                    .Include(lr => lr.Reader)
                    .Where(lr => lr.BookId == bookId)
                    .Select(lr => new
                    {
                        id = lr.Id,
                        readerId = lr.ReaderId,
                        readerName = lr.Reader.Name,
                        lendDate = lr.LendDate,
                        returnDate = lr.ReturnDate
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("searchByReader")]
        public async Task<ActionResult<IEnumerable<object>>> SearchByReader([FromQuery] string keyword)
        {
            try
            {
                var query = _context.LendReturns
                    .Include(lr => lr.Reader)
                    .Include(lr => lr.Book)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // 尝试解析为ID
                    if (int.TryParse(keyword, out int id))
                    {
                        query = query.Where(lr => lr.ReaderId == id);
                    }
                    else
                    {
                        // 按读者姓名搜索
                        keyword = keyword.ToLower();
                        query = query.Where(lr => lr.Reader.Name.ToLower().Contains(keyword));
                    }
                }

                var records = await query
                    .Select(lr => new
                    {
                        id = lr.Id,
                        readerId = lr.ReaderId,
                        readerName = lr.Reader.Name,
                        bookId = lr.BookId,
                        bookName = lr.Book.Name,
                        lendDate = lr.LendDate,
                        returnDate = lr.ReturnDate
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("searchByBook")]
        public async Task<ActionResult<IEnumerable<object>>> SearchByBook([FromQuery] string keyword)
        {
            try
            {
                var query = _context.LendReturns
                    .Include(lr => lr.Reader)
                    .Include(lr => lr.Book)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // 尝试解析为ID
                    if (int.TryParse(keyword, out int id))
                    {
                        query = query.Where(lr => lr.BookId == id);
                    }
                    else
                    {
                        // 按图书名称搜索
                        keyword = keyword.ToLower();
                        query = query.Where(lr => lr.Book.Name.ToLower().Contains(keyword));
                    }
                }

                var records = await query
                    .Select(lr => new
                    {
                        id = lr.Id,
                        readerId = lr.ReaderId,
                        readerName = lr.Reader.Name,
                        bookId = lr.BookId,
                        bookName = lr.Book.Name,
                        lendDate = lr.LendDate,
                        returnDate = lr.ReturnDate
                    })
                    .ToListAsync();

                return Ok(records);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }

    // 添加请求模型
    public class LendRequest
    {
        public int ReaderId { get; set; }
        public int BookId { get; set; }
    }
}