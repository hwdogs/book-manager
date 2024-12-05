using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text;

namespace book_dotnet.Controllers
{
    [ApiController]
    [Route("backup")]
    public class BackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string _backupPath = "Backups";  // 备份文件存储目录

        public BackupController(IConfiguration configuration)
        {
            _configuration = configuration;
            // 确保备份目录存在
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
        }

        [HttpPost("create")]
        public ActionResult<object> CreateBackup()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                var filePath = Path.Combine(_backupPath, fileName);

                // 使用mysqldump命令创建备份
                var mysqldump = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mysqldump",
                        Arguments = $"-h localhost -u root -p123456 book_manager --result-file=\"{filePath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                mysqldump.Start();
                mysqldump.WaitForExit();

                if (mysqldump.ExitCode == 0)
                {
                    return new { status = "success", fileName = fileName };
                }
                else
                {
                    var error = mysqldump.StandardError.ReadToEnd();
                    return new { status = "error", message = error };
                }
            }
            catch (Exception ex)
            {
                return new { status = "error", message = ex.Message };
            }
        }

        [HttpPost("restore")]
        public ActionResult<object> RestoreBackup([FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new { status = "error", message = "No file uploaded" };
                }

                var filePath = Path.Combine(_backupPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // 使用mysql命令恢复备份
                var mysql = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mysql",
                        Arguments = $"-h localhost -u root -p123456 book_manager",
                        UseShellExecute = false,
                        RedirectStandardInput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };

                mysql.Start();
                using (var sw = mysql.StandardInput)
                {
                    sw.WriteLine($"source {filePath}");
                }
                mysql.WaitForExit();

                if (mysql.ExitCode == 0)
                {
                    return new { status = "success" };
                }
                else
                {
                    var error = mysql.StandardError.ReadToEnd();
                    return new { status = "error", message = error };
                }
            }
            catch (Exception ex)
            {
                return new { status = "error", message = ex.Message };
            }
        }

        [HttpGet("list")]
        public ActionResult<object> ListBackups()
        {
            try
            {
                var files = Directory.GetFiles(_backupPath, "backup_*.sql")
                    .Select(f => new FileInfo(f))
                    .Select(f => new
                    {
                        name = f.Name,
                        size = f.Length,
                        createTime = f.CreationTime
                    })
                    .OrderByDescending(f => f.createTime)
                    .ToList();

                return new { status = "success", files = files };
            }
            catch (Exception ex)
            {
                return new { status = "error", message = ex.Message };
            }
        }

        [HttpGet("download/{fileName}")]
        public IActionResult DownloadBackup(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_backupPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound();
                }

                var memory = new MemoryStream();
                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    stream.CopyTo(memory);
                }
                memory.Position = 0;

                return File(memory, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("delete/{fileName}")]
        public ActionResult<object> DeleteBackup(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_backupPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return new { status = "error", message = "文件不存在" };
                }

                System.IO.File.Delete(filePath);
                return new { status = "success" };
            }
            catch (Exception ex)
            {
                return new { status = "error", message = ex.Message };
            }
        }
    }
}