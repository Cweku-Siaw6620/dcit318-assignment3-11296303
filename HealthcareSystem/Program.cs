using System;
using System.Collections.Generic;
using System.Linq;

public class Repository<T>
{
    private List<T> items = new();

    public void Add(T item)
    {
        items.Add(item);
    }

    public List<T> GetAll()
    {
        return items;
    }

    public T? GetById(Func<T, bool> predicate)
    {
        return items.FirstOrDefault(predicate);
    }

    public bool Remove(Func<T, bool> predicate)
    {
        var item = GetById(predicate);
        if (item != null)
        {
            items.Remove(item);
            return true;
        }
        return false;
    }
}

public class Patient
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }
    public string Gender { get; }

    public Patient(int id, string name, int age, string gender)
    {
        Id = id;
        Name = name;
        Age = age;
        Gender = gender;
    }
}

public class Prescription
{
    public int Id { get; }
    public int PatientId { get; }
    public string MedicationName { get; }
    public DateTime DateIssued { get; }

    public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
    {
        Id = id;
        PatientId = patientId;
        MedicationName = medicationName;
        DateIssued = dateIssued;
    }
}

public class HealthSystemApp
{
    private Repository<Patient> _patientRepo = new();
    private Repository<Prescription> _prescriptionRepo = new();
    private Dictionary<int, List<Prescription>> _prescriptionMap = new();

    public void SeedData()
    {
        // Add patients
        _patientRepo.Add(new Patient(1, "Alice Mensah", 28, "Female"));
        _patientRepo.Add(new Patient(2, "Kwame Owusu", 34, "Male"));
        _patientRepo.Add(new Patient(3, "Ama Boateng", 42, "Female"));

        // Add prescriptions
        _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Now.AddDays(-2)));
        _prescriptionRepo.Add(new Prescription(2, 1, "Amoxicillin", DateTime.Now.AddDays(-1)));
        _prescriptionRepo.Add(new Prescription(3, 2, "Ibuprofen", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(4, 3, "Vitamin C", DateTime.Now));
        _prescriptionRepo.Add(new Prescription(5, 2, "Antacid", DateTime.Now));
    }

    public void BuildPrescriptionMap()
    {
        _prescriptionMap = _prescriptionRepo.GetAll()
            .GroupBy(p => p.PatientId)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public void PrintAllPatients()
    {
        Console.WriteLine("All Registered Patients:");
        foreach (var patient in _patientRepo.GetAll())
        {
            Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
        }
    }

    public void PrintPrescriptionsForPatient(int patientId)
    {
        if (_prescriptionMap.TryGetValue(patientId, out var prescriptions))
        {
            Console.WriteLine($"\nPrescriptions for Patient ID {patientId}:");
            foreach (var p in prescriptions)
            {
                Console.WriteLine($"- {p.MedicationName} (Issued: {p.DateIssued.ToShortDateString()})");
            }
        }
        else
        {
            Console.WriteLine("No prescriptions found for this patient.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        HealthSystemApp app = new HealthSystemApp();

        app.SeedData();
        app.BuildPrescriptionMap();
        app.PrintAllPatients();

        Console.Write("\nEnter a Patient ID to view prescriptions: ");
        if (int.TryParse(Console.ReadLine(), out int patientId))
        {
            app.PrintPrescriptionsForPatient(patientId);
        }
        else
        {
            Console.WriteLine("Invalid input.");
        }
    }
}