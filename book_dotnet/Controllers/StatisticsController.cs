using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;
using book_dotnet.Data;
using System.Linq;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly BookDbContext _context;

        public StatisticsController(BookDbContext context)
        {
            _context = context;
        }

        [HttpGet("topBooks")]
        public async Task<ActionResult<object>> GetTopBooks()
        {
            try
            {
                var topBooks = await _context.LendReturns
                    .Include(lr => lr.Book)
                    .GroupBy(lr => lr.BookId)
                    .Select(g => new
                    {
                        bookId = g.Key,
                        bookName = g.First().Book.Name,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToListAsync();

                return Ok(topBooks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTopBooks error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("topReaders")]
        public async Task<ActionResult<object>> GetTopReaders()
        {
            try
            {
                var topReaders = await _context.LendReturns
                    .Include(lr => lr.Reader)
                    .GroupBy(lr => lr.ReaderId)
                    .Select(g => new
                    {
                        readerId = g.Key,
                        readerName = g.First().Reader.Name,
                        count = g.Count()
                    })
                    .OrderByDescending(x => x.count)
                    .Take(10)
                    .ToListAsync();

                return Ok(topReaders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetTopReaders error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("lendTrend")]
        public async Task<ActionResult<object>> GetLendTrend()
        {
            try
            {
                var now = DateTime.Now;
                var startDate = now.AddMonths(-6); // 获取最近6个月的数据

                // 先获取数据再进行格式化
                var trend = await _context.LendReturns
                    .Where(lr => lr.LendDate >= startDate)
                    .GroupBy(lr => new
                    {
                        Year = lr.LendDate.Year,
                        Month = lr.LendDate.Month
                    })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        count = g.Count()
                    })
                    .OrderBy(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                // 在内存中进行日期格式化
                var result = trend.Select(t => new
                {
                    date = $"{t.Year}-{t.Month:D2}",
                    count = t.count
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetLendTrend error: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}