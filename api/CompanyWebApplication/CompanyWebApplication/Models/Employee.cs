using System.Text.Json.Serialization;

namespace CompanyWebApplication.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
       
        public string? FirstName { get; set; }
       
        public string? LastName { get; set; }
        
        public string? EmailAddress { get; set; }
       
        public DateTime DateOfBirth { get; set; }
       
        public int Age { get; set; }
        
        public decimal Salary { get; set; }
       
        public int DepartmentID { get; set; }

        public string? ProfilePhotoName { get; set; }

        // Optional: Navigation property for the associated Department
        //public Department Department { get; set; }
    }
}
