using System.Xml.Serialization;

namespace Medicines.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;
    [XmlType("Pharmacy")]
    public class ImportPharmacyDto
    {
        [XmlElement(nameof(Name))]
        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [XmlElement(nameof(PhoneNumber))]
        [Required]
        [RegularExpression(@"\(\d{3}\) \d{3}-\d{4}")]
        public string PhoneNumber { get; set; } = null!;

        [XmlAttribute("non-stop")] 
        [Required] 
        public string IsNonStop { get; set; } = null!;

        public ImportMedicineDto[] Medicines { get; set; } = null!;
    }
}
