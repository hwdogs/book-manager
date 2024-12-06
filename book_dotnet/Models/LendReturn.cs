using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace book_dotnet.Models
{
    [Table("lend_return")]
    public class LendReturn
    {
        public LendReturn()
        {
            Reader = null!;
            Book = null!;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("reader_id")]
        public int ReaderId { get; set; }

        [Required]
        [Column("book_id")]
        public int BookId { get; set; }

        [Required]
        [Column("lend_data")]
        public DateTime LendDate { get; set; }

        [Column("return_data")]
        public DateTime? ReturnDate { get; set; }

        [ForeignKey("ReaderId")]
        public virtual Reader Reader { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }
    }
}