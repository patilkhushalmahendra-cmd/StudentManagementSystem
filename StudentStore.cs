using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StudentManagementSystem
{
    public class StudentStore
    {
        private readonly string filePath;
        public StudentStore()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CampusDesk");
            Directory.CreateDirectory(folder);
            filePath = Path.Combine(folder, "students.xml");
        }

        public List<Student> Load()
        {
            if (!File.Exists(filePath)) return SeedData();
            try
            {
                using (var stream = File.OpenRead(filePath))
                    return (List<Student>)new XmlSerializer(typeof(List<Student>)).Deserialize(stream);
            }
            catch
            {
                return SeedData();
            }
        }

        public void Save(List<Student> students)
        {
            using (var stream = File.Create(filePath))
                new XmlSerializer(typeof(List<Student>)).Serialize(stream, students);
        }

        private List<Student> SeedData()
        {
            return new List<Student>
            {
                new Student { Id=1001, FullName="Aarav Shah", Course="BCA", Email="aarav@example.com", Phone="9876543210", Semester=5, Percentage=86.5, Status="Active" },
                new Student { Id=1002, FullName="Diya Patel", Course="BSc IT", Email="diya@example.com", Phone="9876501234", Semester=3, Percentage=91.0, Status="Active" },
                new Student { Id=1003, FullName="Kabir Mehta", Course="BCA", Email="kabir@example.com", Phone="9898989898", Semester=4, Percentage=74.5, Status="Active" },
                new Student { Id=1004, FullName="Anaya Desai", Course="BCom", Email="anaya@example.com", Phone="9812345670", Semester=2, Percentage=68.0, Status="Inactive" }
            };
        }
    }
}
