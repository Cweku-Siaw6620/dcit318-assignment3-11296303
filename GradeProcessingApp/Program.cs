using System;
using System.Collections.Generic;
using System.IO;

// Custom exception classes
public class InvalidScoreFormatException : Exception
{
    public InvalidScoreFormatException(string message) : base(message) { }
}

public class MissingFieldException : Exception
{
    public MissingFieldException(string message) : base(message) { }
}

// Student class
public class Student
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public int Score { get; set; }

    public string GetGrade()
    {
        if (Score >= 80 && Score <= 100) return "A";
        if (Score >= 70 && Score <= 79) return "B";
        if (Score >= 60 && Score <= 69) return "C";
        if (Score >= 50 && Score <= 59) return "D";
        return "F";
    }
}

// Student result processor class
public class StudentResultProcessor
{
    public List<Student> ReadStudentsFromFile(string inputFilePath)
    {
        List<Student> students = new List<Student>();

        using (StreamReader reader = new StreamReader(inputFilePath))
        {
            int lineNumber = 0;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                string line = reader.ReadLine();
                string[] fields = line.Split(',');

                // Validate fields
                if (fields.Length != 3)
                {
                    throw new MissingFieldException($"Line {lineNumber}: Expected 3 fields but found {fields.Length}");
                }

                // Parse ID
                if (!int.TryParse(fields[0].Trim(), out int id))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid ID format '{fields[0]}'");
                }

                // Parse score
                if (!int.TryParse(fields[2].Trim(), out int score))
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score format '{fields[2]}'");
                }

                // Validate score range
                if (score < 0 || score > 100)
                {
                    throw new InvalidScoreFormatException($"Line {lineNumber}: Score must be between 0 and 100");
                }

                students.Add(new Student
                {
                    Id = id,
                    FullName = fields[1].Trim(),
                    Score = score
                });
            }
        }

        return students;
    }

    public void WriteReportToFile(List<Student> students, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            foreach (var student in students)
            {
                string line = $"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}";
                writer.WriteLine(line);
            }
        }
    }
}

// Main application
class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Student Grading System");
            Console.WriteLine("----------------------");
            
            Console.Write("Enter input file path: ");
            string inputFile = Console.ReadLine();
            
            Console.Write("Enter output file path: ");
            string outputFile = Console.ReadLine();

            var processor = new StudentResultProcessor();
            var students = processor.ReadStudentsFromFile(inputFile);
            processor.WriteReportToFile(students, outputFile);

            Console.WriteLine($"Successfully processed {students.Count} students. Report written to {outputFile}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: Input file not found - {ex.Message}");
        }
        catch (InvalidScoreFormatException ex)
        {
            Console.WriteLine($"Error: Invalid score format - {ex.Message}");
        }
        catch (MissingFieldException ex)
        {
            Console.WriteLine($"Error: Missing field in record - {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
    }
}