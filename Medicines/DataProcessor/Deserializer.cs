using Medicines.Data.Models;
using Medicines.Data.Models.Enums;
using Medicines.DataProcessor.ImportDtos;
using Medicines.Utilities;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;

namespace Medicines.DataProcessor
{
    using Medicines.Data;
    using System.ComponentModel.DataAnnotations;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid Data!";
        private const string SuccessfullyImportedPharmacy = "Successfully imported pharmacy - {0} with {1} medicines.";
        private const string SuccessfullyImportedPatient = "Successfully imported patient - {0} with {1} medicines.";

        public static string ImportPatients(MedicinesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportJsonPatients[]? importJsonPatientsDto =
                JsonConvert.DeserializeObject<ImportJsonPatients[]>(jsonString);

            if (importJsonPatientsDto != null && importJsonPatientsDto.Length > 0)
            {
                ICollection<Patient> patientsToAdd = new List<Patient>();

                foreach (ImportJsonPatients importJsonPatients in importJsonPatientsDto)
                {
                    if (!IsValid(importJsonPatients))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (importJsonPatients.AgeGroup < 0 || importJsonPatients.AgeGroup > 2)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if (importJsonPatients.Gender < 0 || importJsonPatients.Gender > 1)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Patient patient = new Patient()
                    {
                        FullName = importJsonPatients.FullName,
                        AgeGroup = (AgeGroup)importJsonPatients.AgeGroup,
                        Gender = (Gender)importJsonPatients.Gender,

                    };

                    foreach (int medicineId in importJsonPatients.Medicines)
                    {
                        if (patient.PatientsMedicines.Any(pm => pm.MedicineId == medicineId))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        PatientMedicine patientMedicine = new PatientMedicine()
                        {
                            Patient = patient,
                            MedicineId = medicineId
                        };

                        patient.PatientsMedicines.Add(patientMedicine);
                    }

                    patientsToAdd.Add(patient);
                    sb.AppendLine(string.Format(SuccessfullyImportedPatient, patient.FullName,
                        patient.PatientsMedicines.Count));
                }
                context.AddRange(patientsToAdd);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        public static string ImportPharmacies(MedicinesContext context, string xmlString)
        {
            
            StringBuilder sb = new StringBuilder();

            ImportPharmacyDto[]? importPharmacyDtos =
                XmlHelper.Deserialize<ImportPharmacyDto[]>(xmlString, "Pharmacies");

            if (importPharmacyDtos != null && importPharmacyDtos.Length > 0)
            {
                ICollection<Pharmacy> pharmaciesToAdd = new List<Pharmacy>();

                foreach (ImportPharmacyDto importPharmacyDto in importPharmacyDtos)
                {
                    if (!IsValid(importPharmacyDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isValidBoolean = bool.TryParse(importPharmacyDto.IsNonStop, out bool parsedNonStop);

                    if (!isValidBoolean)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ICollection<Medicine> medicinesToAdd = new List<Medicine>();

                    foreach (ImportMedicineDto importMedicineDto in importPharmacyDto.Medicines)
                    {
                        if (!IsValid(importMedicineDto))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        bool isValidProductionDate = DateTime.TryParseExact(importMedicineDto.ProductionDate, "yyyy-MM-dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedProductionDate);

                        if (!isValidProductionDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        bool isValidExpirationDate = DateTime.TryParseExact(importMedicineDto.ExpiryDate, "yyyy-MM-dd",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedExpiryDate);

                        if (!isValidExpirationDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (parsedProductionDate >= parsedExpiryDate)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (importMedicineDto.Category < 0 || importMedicineDto.Category > 4)
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        if (medicinesToAdd.Any(m => m.Name == importMedicineDto.Name && m.Producer == importMedicineDto.Producer))
                        {
                            sb.AppendLine(ErrorMessage);
                            continue;
                        }

                        Medicine medicine = new Medicine()
                        {
                            Category = (Category)importMedicineDto.Category,
                            Name = importMedicineDto.Name,
                            Price = importMedicineDto.Price,
                            ProductionDate = parsedProductionDate,
                            ExpiryDate = parsedExpiryDate,
                            Producer = importMedicineDto.Producer
                        };

                        medicinesToAdd.Add(medicine);
                    }

                    Pharmacy pharmacy = new Pharmacy()
                    {
                        IsNonStop = parsedNonStop,
                        Name = importPharmacyDto.Name,
                        PhoneNumber = importPharmacyDto.PhoneNumber,
                        Medicines = medicinesToAdd
                    };
                    sb.AppendLine(string.Format(SuccessfullyImportedPharmacy, pharmacy.Name, pharmacy.Medicines.Count));
                    pharmaciesToAdd.Add(pharmacy);
                }
                context.AddRange(pharmaciesToAdd);
                context.SaveChanges();
            }

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
