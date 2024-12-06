using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text;

namespace book_dotnet.Controllers
{
    /// <summary>
    /// ���ݿⱸ����ָ�������
    /// </summary>
    [ApiController]
    [Route("backup")]
    public class BackupController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        // �����ļ��洢Ŀ¼
        private readonly string _backupPath = "Backups";

        public BackupController(IConfiguration configuration)
        {
            _configuration = configuration;
            // ȷ������Ŀ¼����
            if (!Directory.Exists(_backupPath))
            {
                Directory.CreateDirectory(_backupPath);
            }
        }

        /// <summary>
        /// �������ݿⱸ��
        /// </summary>
        /// <returns>��������״̬���ļ����Ķ���</returns>
        [HttpPost("create")]
        public ActionResult<object> CreateBackup()
        {
            try
            {
                // �������ļ���ȡ���ݿ�������Ϣ
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var builder = new MySqlConnectionStringBuilder(connectionString);
                // ���ɱ����ļ�������ʽ��backup_������_ʱ����.sql
                var fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                var filePath = Path.Combine(_backupPath, fileName);

                // ʹ��mysqldump���������
                var mysqldump = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "mysqldump",
                        // ʹ�������ļ��е����ݿ�������Ϣ
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
                    // �ڱ����ļ���ͷ������ݿ���Ϣ�����ں�����֤
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
        /// �ӱ����ļ��ָ����ݿ�
        /// </summary>
        /// <param name="file">�ϴ��ı����ļ�</param>
        /// <returns>�ָ�������״̬</returns>
        [HttpPost("restore")]
        public ActionResult<object> RestoreBackup([FromForm] IFormFile file)
        {
            try
            {
                // ��֤�ϴ����ļ�
                if (file == null || file.Length == 0)
                {
                    return new { status = "error", message = "No file uploaded" };
                }

                // �����ϴ����ļ�
                var filePath = Path.Combine(_backupPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // ��֤�����ļ���ʽ
                var firstLine = System.IO.File.ReadLines(filePath).First();
                if (!firstLine.StartsWith("-- Database:"))
                {
                    return new { status = "error", message = "Invalid backup file format" };
                }

                // �������ļ���ȡ���ݿ�������Ϣ
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                var builder = new MySqlConnectionStringBuilder(connectionString);

                // ʹ��mysql����ָ�����
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
        /// ��ȡ���б����ļ��б�
        /// </summary>
        /// <returns>�����ļ��б������ļ�������С�ʹ���ʱ��</returns>
        [HttpGet("list")]
        public ActionResult<object> ListBackups()
        {
            try
            {
                // ��ȡ����.sql�����ļ���������ʱ�併������
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
        /// ����ָ���ı����ļ�
        /// </summary>
        /// <param name="fileName">�����ļ���</param>
        /// <returns>�����ļ����ļ���</returns>
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

                // ���ļ������ڴ���������
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
        /// ɾ��ָ���ı����ļ�
        /// </summary>
        /// <param name="fileName">�����ļ���</param>
        /// <returns>ɾ��������״̬</returns>
        [HttpDelete("delete/{fileName}")]
        public ActionResult<object> DeleteBackup(string fileName)
        {
            try
            {
                var filePath = Path.Combine(_backupPath, fileName);
                if (!System.IO.File.Exists(filePath))
                {
                    return new { status = "error", message = "�ļ�������" };
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