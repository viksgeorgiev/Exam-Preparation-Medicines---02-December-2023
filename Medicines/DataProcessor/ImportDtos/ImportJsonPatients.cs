namespace Medicines.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    public class ImportJsonPatients
    {
        [Required]
        [MinLength(5)]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required] 
        public int AgeGroup { get; set; }

        [Required] 
        public int Gender { get; set; } 

        public int[] Medicines { get; set; } = null!;

    }
}
