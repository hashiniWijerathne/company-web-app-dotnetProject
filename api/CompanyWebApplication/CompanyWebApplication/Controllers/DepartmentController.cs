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
    public class DepartmentController : ControllerBase
    {
        // Dependency injection for IConfiguration to read the connection string
        private IConfiguration _configuration;
        // use the logger
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(IConfiguration configuration, ILogger<DepartmentController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        // GET: api/Department
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Retrieve all department details from the database
                string query = "SELECT * FROM dbo.Department";
                DataTable table = new DataTable();
                string? sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        SqlDataReader myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                    }

                    myCon.Close();
                }

                // Return the department details in JSON format
                return new JsonResult(table);
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error response
                _logger.LogError(ex, "An error occurred while processing the GET request for departments.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error");
            }
        }

        // POST: api/Department
        [HttpPost]
        public JsonResult Post(Department dep)
        {
            try
            {
                // Insert a new department into the database
                string query = @"
                   INSERT INTO dbo.Department (DepartmentCode, DepartmentName)
                   VALUES (@DepartmentCode, @DepartmentName)
                ";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        // Set parameters for the SQL query
                        myCommand.Parameters.AddWithValue("@DepartmentCode", dep.DepartmentCode);
                        myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);

                        SqlDataReader myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                    }

                    myCon.Close();
                }

                // Log a message
                _logger.LogInformation("Retrieved all department details successfully.");
                // Return a success message
                return new JsonResult("Added Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error response
                // Log.Error(ex, "An error occurred while processing the POST request for departments.");
                return new JsonResult(new { ErrorMessage = "Internal server error" }) { StatusCode = StatusCodes.Status500InternalServerError };


            }
        }

        // PUT: api/Department
        [HttpPut]
        public JsonResult Put(Department dep)
        {
            try
            {
                // Update an existing department in the database
                string query = @"
                   UPDATE dbo.Department
                   SET DepartmentCode = @DepartmentCode,
                       DepartmentName = @DepartmentName
                   WHERE DepartmentId = @DepartmentId
                ";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    using (SqlCommand myCommand = new SqlCommand(query, myCon))
                    {
                        // Set parameters for the SQL query
                        myCommand.Parameters.AddWithValue("@DepartmentId", dep.DepartmentId);
                        myCommand.Parameters.AddWithValue("@DepartmentCode", dep.DepartmentCode);
                        myCommand.Parameters.AddWithValue("@DepartmentName", dep.DepartmentName);

                        SqlDataReader myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                    }

                    myCon.Close();
                }

                // Return a success message
                return new JsonResult("Updated Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error response
                // Log.Error(ex, "An error occurred while processing the PUT request for departments.");
                return new JsonResult(new { ErrorMessage = "Internal server error" }) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }

        // DELETE: api/Department/5
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            try
            {
                // Delete a department and its associated employees in a cascade manner

                // Step 1: SQL query to get associated EmployeeIds in the specified Department
                string queryGetEmployees = "SELECT EmployeeId FROM dbo.Employee WHERE DepartmentId = @DepartmentId";

                // Step 2: SQL query to delete associated Employees in the specified Department
                string queryDeleteEmployees = "DELETE FROM dbo.Employee WHERE DepartmentId = @DepartmentId";

                // Step 3: SQL query to delete the specified Department
                string queryDeleteDepartment = "DELETE FROM dbo.Department WHERE DepartmentId = @DepartmentId";

                DataTable employeesTable = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");

                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
                {
                    myCon.Open();

                    // Step 1: Execute the first query to get associated EmployeeIds
                    using (SqlCommand getEmployeesCommand = new SqlCommand(queryGetEmployees, myCon))
                    {
                        getEmployeesCommand.Parameters.AddWithValue("@DepartmentId", id);

                        // Use SqlDataAdapter to fill the DataTable with the result
                        SqlDataAdapter employeesAdapter = new SqlDataAdapter(getEmployeesCommand);
                        employeesAdapter.Fill(employeesTable);
                    }

                    // Step 2: Delete associated Employees using the second query
                    using (SqlCommand deleteEmployeesCommand = new SqlCommand(queryDeleteEmployees, myCon))
                    {
                        deleteEmployeesCommand.Parameters.AddWithValue("@DepartmentId", id);
                        deleteEmployeesCommand.ExecuteNonQuery();
                    }

                    // Step 3: Delete the specified Department using the third query
                    using (SqlCommand deleteDepartmentCommand = new SqlCommand(queryDeleteDepartment, myCon))
                    {
                        deleteDepartmentCommand.Parameters.AddWithValue("@DepartmentId", id);
                        deleteDepartmentCommand.ExecuteNonQuery();
                    }

                    myCon.Close();
                }

                // Return a success message
                return new JsonResult("Cascade Deleted Successfully");
            }
            catch (Exception ex)
            {
                // Log the exception and return an internal server error response
                // Log.Error(ex, "An error occurred while processing the DELETE request for departments.");
                return new JsonResult(new { ErrorMessage = "Internal server error" }) { StatusCode = StatusCodes.Status500InternalServerError };

            }
        }
    }
}
