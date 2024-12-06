using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text;

namespace book_dotnet.Controllers
{
    /// <summary>
    /// 数据库备份与恢复控制器
    /// </summary>
    [ApiController]
    [Route("backup")]
    public class BackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        // 备份文件存储目录
        private readonly string _backupPath = "Backups";

        public BackupController(IConfiguration configuration)
        {
            _configuration = configuration;
            // 确保备份目录存在
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
        }

        /// <summary>
        /// 创建数据库备份
        /// </summary>
        /// <returns>包含备份状态和文件名的对象</returns>
        [HttpPost("create")]
        public ActionResult<object> CreateBackup()
        {
            try
            {
                // 从配置文件获取数据库连接信息
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var builder = new MySqlConnectionStringBuilder(connectionString);
                // 生成备份文件名，格式：backup_年月日_时分秒.sql
                var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                var filePath = Path.Combine(_backupPath, fileName);

                // 使用mysqldump命令创建备份
                var mysqldump = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mysqldump",
                        // 使用配置文件中的数据库连接信息
                        Arguments = $"-h {builder.Server} -u {builder.UserID} -p{builder.Password} {builder.Database} --result-file=\"{filePath}\"",
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
                    // 在备份文件开头添加数据库信息，用于后续验证
                    var dbInfo = $"-- Database: {builder.Database}\n";
                    var fileContent = System.IO.File.ReadAllText(filePath);
                    System.IO.File.WriteAllText(filePath, dbInfo + fileContent);

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

        /// <summary>
        /// 从备份文件恢复数据库
        /// </summary>
        /// <param name="file">上传的备份文件</param>
        /// <returns>恢复操作的状态</returns>
        [HttpPost("restore")]
        public ActionResult<object> RestoreBackup([FromForm] IFormFile file)
        {
            try
            {
                // 验证上传的文件
                if (file == null || file.Length == 0)
                {
                    return new { status = "error", message = "No file uploaded" };
                }

                // 保存上传的文件
                var filePath = Path.Combine(_backupPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // 验证备份文件格式
                var firstLine = System.IO.File.ReadLines(filePath).First();
                if (!firstLine.StartsWith("-- Database:"))
                {
                    return new { status = "error", message = "Invalid backup file format" };
                }

                // 从配置文件获取数据库连接信息
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var builder = new MySqlConnectionStringBuilder(connectionString);

                // 使用mysql命令恢复备份
                var mysql = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mysql",
                        Arguments = $"-h {builder.Server} -u {builder.UserID} -p{builder.Password} {builder.Database}",
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

        /// <summary>
        /// 获取所有备份文件列表
        /// </summary>
        /// <returns>备份文件列表，包含文件名、大小和创建时间</returns>
        [HttpGet("list")]
        public ActionResult<object> ListBackups()
        {
            try
            {
                // 获取所有.sql备份文件并按创建时间降序排序
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

        /// <summary>
        /// 下载指定的备份文件
        /// </summary>
        /// <param name="fileName">备份文件名</param>
        /// <returns>备份文件的文件流</returns>
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

                // 将文件读入内存流并返回
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

        /// <summary>
        /// 删除指定的备份文件
        /// </summary>
        /// <param name="fileName">备份文件名</param>
        /// <returns>删除操作的状态</returns>
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