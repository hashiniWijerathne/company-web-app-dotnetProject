using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CompanyWebApplication.Models;

namespace CompanyWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        //to read the connection string - make use of dependency injection
        private readonly IConfiguration _configuration;

        //provide info about hosting environment - use to upload photos int the folder directory
        private readonly IWebHostEnvironment _env;
        // Constructor to initialize the controller with IConfiguration and IWebHostEnvironment
        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get()
        {
            // SQL query to join Employee and Department tables and retrieve data
            string query = "SELECT e.*, d.DepartmentName FROM dbo.Employee e JOIN dbo.Department d ON e.DepartmentID = d.DepartmentId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;

            // Establish a connection to the SQL Server database
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                // Execute SQL query using a SqlCommand
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    // ExecuteReader() retrieves the data from the database
                    myReader = myCommand.ExecuteReader();

                    // Load the retrieved data into the DataTable
                    table.Load(myReader);

                    // Close the SqlDataReader and SqlConnection to release resources
                    myReader.Close();
                    myCon.Close();
                }
            }

            // Return the retrieved data as a JsonResult
            return new JsonResult(table);
        }



        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            // SQL query to insert employee data into the Employee table
            string query = @"
           INSERT INTO dbo.Employee (FirstName, LastName, EmailAddress, DateOfBirth, Age, Salary, DepartmentID, ProfilePhotoName)
           VALUES (@FirstName, @LastName, @EmailAddress, @DateOfBirth, @Age, @Salary, @DepartmentID, @ProfilePhotoName)
        ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;

            // Establish a connection to the SQL Server database
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                // Create a SqlCommand with parameters for the INSERT query
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    myCommand.Parameters.AddWithValue("@LastName", emp.LastName);
                    myCommand.Parameters.AddWithValue("@EmailAddress", emp.EmailAddress);
                    myCommand.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                    myCommand.Parameters.AddWithValue("@Age", emp.Age);
                    myCommand.Parameters.AddWithValue("@Salary", emp.Salary);
                    myCommand.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    myCommand.Parameters.AddWithValue("@ProfilePhotoName", emp.ProfilePhotoName);

                    // Execute the INSERT query
                    myReader = myCommand.ExecuteReader();

                    // Load the result into the DataTable
                    table.Load(myReader);

                    // Close the SqlDataReader and SqlConnection to release resources
                    myReader.Close();
                    myCon.Close();
                }
            }

            // Return a JsonResult indicating the operation success
            return new JsonResult("Added Successfully");
        }


        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            // SQL query to update employee data in the Employee table based on EmployeeID
            string query = @"
           UPDATE dbo.Employee
           SET FirstName = @FirstName,
               LastName = @LastName,
               EmailAddress = @EmailAddress,
               DateOfBirth = @DateOfBirth,
               Age = @Age,
               Salary = @Salary,
               DepartmentID = @DepartmentID,
               ProfilePhotoName = @ProfilePhotoName
           WHERE EmployeeID = @EmployeeID
        ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;

            // Establish a connection to the SQL Server database
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();

                // Create a SqlCommand with parameters for the UPDATE query
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeID", emp.EmployeeID);
                    myCommand.Parameters.AddWithValue("@FirstName", emp.FirstName);
                    myCommand.Parameters.AddWithValue("@LastName", emp.LastName);
                    myCommand.Parameters.AddWithValue("@EmailAddress", emp.EmailAddress);
                    myCommand.Parameters.AddWithValue("@DateOfBirth", emp.DateOfBirth);
                    myCommand.Parameters.AddWithValue("@Age", emp.Age);
                    myCommand.Parameters.AddWithValue("@Salary", emp.Salary);
                    myCommand.Parameters.AddWithValue("@DepartmentID", emp.DepartmentID);
                    myCommand.Parameters.AddWithValue("@ProfilePhotoName", emp.ProfilePhotoName);

                    // Execute the UPDATE query
                    myReader = myCommand.ExecuteReader();

                    // Load the result into the DataTable
                    table.Load(myReader);

                    // Close the SqlDataReader and SqlConnection to release resources
                    myReader.Close();
                    myCon.Close();
                }
            }

            // Return a JsonResult indicating the operation success
            return new JsonResult("Updated Successfully");
        }


        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                           DELETE FROM dbo.Employee
                           WHERE EmployeeID = @EmployeeID
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeID", id);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public async Task<IActionResult> SaveFile()
        {
            try
            {
                // Read the form data asynchronously
                var formCollection = await Request.ReadFormAsync();

                // Assuming only one file is uploaded
                var file = formCollection.Files.First();

                if (file.Length > 0)
                {
                    // Get the original file name and create a file path
                    var fileName = file.FileName;
                    var filePath = Path.Combine(_env.ContentRootPath, "Photos", fileName);

                    // Create a FileStream to write the uploaded file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        // Copy the contents of the file to the FileStream
                        await file.CopyToAsync(stream);
                    }

                    // Return the uploaded file name as a JsonResult
                    return new JsonResult(fileName);
                }
                else
                {
                    // Return a JsonResult indicating that the file was not found
                    return new JsonResult("File not found");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (this should be improved to provide more detailed logging)
                return new JsonResult("Error during file upload");
            }
        }


    }
}
