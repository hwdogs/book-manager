using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_dotnet.Models
{
    [Table("reader")]
    public class Reader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("sex")]
        public bool Sex { get; set; }  // tinyint ÔÚC#ÖĞÓ³ÉäÎªbool

        [Required]
        [Column("w_address")]
        public string WorkAddress { get; set; }

        [Required]
        [Column("h_address")]
        public string HomeAddress { get; set; }

        [Column("email")]
        public string? Email { get; set; }

        [Required]
        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }
    }
}