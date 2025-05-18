using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationAPI.Infrastructure.Entities
{
    [Table("token")]
    public class Token
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [MaxLength(250)]
        public string RefreshToken { get; set; }

        [Required]
        [MaxLength(64)]
        public string ExpiresIn { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
    }
}
