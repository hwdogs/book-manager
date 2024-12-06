using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_dotnet.Models
{
    [Table("manager")]
    public class Manager
    {
        public Manager()
        {
            Name = string.Empty;
            Password = string.Empty;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("password")]
        public string Password { get; set; }
    }
}