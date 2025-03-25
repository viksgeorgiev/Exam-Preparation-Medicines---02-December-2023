using System.Xml.Serialization;
using Medicines.Data.Models;

namespace Medicines.DataProcessor.ExportDtos
{
    [XmlType("Patient")]
    public class ExportPatientsDto
    {
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlElement(nameof(AgeGroup))]
        public string AgeGroup { get; set; } = null!;

        [XmlAttribute(nameof(Gender))]
        public string Gender { get; set; } = null!;

        [XmlArray("Medicines")]
        public ExportMedicineDto[] Medicines { get; set; } = null!;
    }
}
