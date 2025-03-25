using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    [XmlType("Medicine")]
    public class ImportMedicineDto
    {
        [XmlElement(nameof(Name))]
        [Required]
        [MinLength(3)]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [XmlElement(nameof(Price))]
        [Required]
        [Range(typeof(decimal), "0.01", "1000.00")]
        public decimal Price { get; set; }

        [XmlAttribute("category")]
        [Required] 
        public int Category { get; set; }

        [XmlElement(nameof(ProductionDate))]
        [Required] 
        public string ProductionDate { get; set; } = null!;

        [XmlElement(nameof(ExpiryDate))]
        [Required]
        public string ExpiryDate { get; set; } = null!;

        [XmlElement(nameof(Producer))]
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Producer { get; set; } = null!;

        
    }
}