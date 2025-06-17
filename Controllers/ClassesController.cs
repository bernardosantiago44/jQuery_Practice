using jQuery_Practice.Data;
using jQuery_Practice.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Serilog;

namespace jQuery_Practice.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ClassesController : ControllerBase
    {
        private IConfiguration _configuration;
        private string _connectionString;

        public ClassesController(IConfiguration configuration)
        {
            _configuration = configuration;
            try
            {
                setupConnectionString();
            }
            catch (Exception exception) {
                Log.Error(exception.Message);
            }

        }

        private void setupConnectionString()
        {
            if (_configuration == null)
            {
                throw new Exception("No configuration found.");
            }
            string connection = _configuration.GetConnectionString("DefaultConnection");

            if (connection == null)
            {
                throw new Exception("Connection string DefaultConnection is not configured");
            }

            _connectionString = connection;
        }

        [HttpPost("all")]
        public async Task<ActionResult<IEnumerable<Class>>> GetAll()
        {
            var classes = new List<Class>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT * 
                FROM Classes    
                ORDER BY Id";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var classObj = new Class
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Instructor = reader.GetString(2),
                    AcademicCredits = reader.GetInt32(3)
                };
                classes.Add(classObj);
            }

            return classes;
        }
    
        [HttpPost("create")]
        public async Task<ActionResult<Class>> Create(Class newClass)
        {
            int newId;

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Classes (Name, Instructor, AcademicCredits)
                VALUES (@Name, @Instructor, @AcademicCredits);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Name", newClass.Name);
            command.Parameters.AddWithValue("@Instructor", newClass.Instructor);
            command.Parameters.AddWithValue("@AcademicCredits", newClass.AcademicCredits);

            newId = await command.ExecuteNonQueryAsync();
            newClass.Id = newId;

            return Ok(newClass);
        }
    }
}
