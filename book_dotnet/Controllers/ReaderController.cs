using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using book_dotnet.Models;
using book_dotnet.Data;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("reader")]
    public class ReaderController : ControllerBase
    {
        private readonly BookDbContext _context;

        public ReaderController(BookDbContext context)
        {
            _context = context;
        }

        [HttpGet("findAll/{page}/{size}")]
        public async Task<ActionResult<object>> FindAll(int page, int size)
        {
            try
            {
                var totalElements = await _context.Readers.CountAsync();
                var readers = await _context.Readers
                    .OrderBy(r => r.Id)  // 改为正序排列
                    .Skip(page * size)
                    .Take(size)
                    .Select(r => new  // 确保属性名称与前端匹配
                    {
                        id = r.Id,
                        name = r.Name,
                        sex = r.Sex,
                        workAddress = r.WorkAddress,
                        homeAddress = r.HomeAddress,
                        email = r.Email,
                        createTime = r.CreateTime,
                        notes = r.Notes
                    })
                    .ToListAsync();

                var result = new
                {
                    content = readers,
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
                    numberOfElements = readers.Count,
                    first = page == 0,
                    empty = !readers.Any()
                };

                return Ok(result);  // 使用Ok()包装返回结果
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("findById/{id}")]
        public async Task<ActionResult<Reader>> FindById(int id)
        {
            var reader = await _context.Readers.FindAsync(id);

            if (reader == null)
            {
                return NotFound();
            }

            return reader;
        }

        [HttpPost("save")]
        public async Task<ActionResult<string>> Save(Reader reader)
        {
            reader.CreateTime = DateTime.Now;  // 设置创建时间
            _context.Readers.Add(reader);
            var result = await _context.SaveChangesAsync();

            return result > 0 ? "success" : "error";
        }

        [HttpPut("update")]
        public async Task<ActionResult<string>> Update([FromBody] Reader reader)
        {
            try
            {
                var existingReader = await _context.Readers.FindAsync(reader.Id);
                if (existingReader == null)
                {
                    return "error";
                }

                // 更新读者信息，但保留原创建时间
                reader.CreateTime = existingReader.CreateTime;
                _context.Entry(existingReader).CurrentValues.SetValues(reader);

                await _context.SaveChangesAsync();
                return "success";
            }
            catch (Exception)
            {
                return "error";
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult<string>> Delete(int id)
        {
            var reader = await _context.Readers.FindAsync(id);
            if (reader == null)
            {
                return "error";
            }

            _context.Readers.Remove(reader);
            var result = await _context.SaveChangesAsync();

            return result > 0 ? "success" : "error";
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Reader>>> Search([FromQuery] string keyword)
        {
            try
            {
                var query = _context.Readers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    // 尝试解析为ID
                    if (int.TryParse(keyword, out int id))
                    {
                        query = query.Where(r => r.Id == id);
                    }
                    else
                    {
                        // 按姓名或地址搜索
                        keyword = keyword.ToLower();
                        query = query.Where(r =>
                            r.Name.ToLower().Contains(keyword) ||
                            r.WorkAddress.ToLower().Contains(keyword) ||
                            r.HomeAddress.ToLower().Contains(keyword));
                    }
                }

                var readers = await query.Take(10).ToListAsync();
                return Ok(readers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}