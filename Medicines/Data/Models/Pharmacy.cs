using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Medicines.Data.Models
{
    public class Pharmacy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [Required]
        [Column(TypeName = "VARCHAR(14)")]
        [RegularExpression(@"\(\d{3}\) \d{3}-\d{4}")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public bool IsNonStop { get; set; }

        public virtual ICollection<Medicine> Medicines { get; set; } 
            = new HashSet<Medicine>();
    }
}
