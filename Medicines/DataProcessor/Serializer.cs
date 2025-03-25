using Medicines.DataProcessor.ExportDtos;
using Medicines.Utilities;
using System.Globalization;
using Medicines.Data.Models.Enums;
using Newtonsoft.Json;

namespace Medicines.DataProcessor
{
    using Medicines.Data;

    public class Serializer
    {
        public static string ExportPatientsWithTheirMedicines(MedicinesContext context, string date)
        {
            DateTime givenDate;

            if (!DateTime.TryParse(date, out givenDate))
            {
                throw new ArgumentException("Invalid date format!");
            }

            var patients = context.Patients
                .Where(p => p.PatientsMedicines.Any(pm => pm.Medicine.ProductionDate > givenDate))
                .Select(p => new ExportPatientsDto
                {
                    Name = p.FullName,
                    Gender = p.Gender.ToString().ToLower(),
                    AgeGroup = p.AgeGroup.ToString(),
                    Medicines = p.PatientsMedicines
                        .Where(pm => pm.Medicine.ProductionDate > givenDate)
                        .Select(pm => pm.Medicine)
                        .OrderByDescending(m => m.ExpiryDate)
                        .ThenBy(m => m.Price)
                        .Select(m => new ExportMedicineDto
                        {
                            Name = m.Name,
                            Price = m.Price.ToString("F2", CultureInfo.InvariantCulture),
                            Category = m.Category.ToString().ToLower(),
                            Producer = m.Producer,
                            BestBefore = m.ExpiryDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
                        })
                        .ToArray()
                })
                .OrderByDescending(p => p.Medicines.Length)
                .ThenBy(p => p.Name)
                .ToArray();

            string result = XmlHelper.Serialize(patients, "Patients");
            return result;
        }

        public static string ExportMedicinesFromDesiredCategoryInNonStopPharmacies(MedicinesContext context, int medicineCategory)
        {
            var medicines = context
                .Medicines
                .Where(m => m.Category == (Category)medicineCategory && m.Pharmacy.IsNonStop == true)
                .OrderBy(m => m.Price)
                .ThenBy(m => m.Name)
                .Select(m => new
                {
                    Name = m.Name,
                    Price = m.Price.ToString("F2"),
                    Pharmacy = new
                    {
                        Name = m.Pharmacy.Name,
                        PhoneNumber = m.Pharmacy.PhoneNumber
                    }
                })
                .ToArray();

            string result = JsonConvert.SerializeObject(medicines,Formatting.Indented);

            return result;
        }
    }
}
