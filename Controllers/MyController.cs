using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;

namespace CSDS_Assign_1.Controllers
{
    public class MyController : ControllerBase
    {
        private readonly Repository _repository;

        // Inject Repository via Constructor
        public MyController(Repository repository)
        {
            _repository = repository;
        }

        // Serve the Categories HTML page
        [HttpGet("categories")]
        public IActionResult Categories()
        {
            return GetHtmlFile("categories");
        }

        // Retrieve the list of categories
        [HttpGet("categoriesFromDB")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _repository.GetCategories();
                return Ok(categories); // Return 200 OK with the categories list
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        // Serve an HTML file
        [HttpGet("html/{str}")]
        public IActionResult GetHtmlFile(string str)
        {
            // Sanitize the input to prevent directory traversal
            var sanitizedStr = Path.GetFileNameWithoutExtension(str);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "HTML", sanitizedStr + ".html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"The file '{sanitizedStr}.html' was not found.");
            }

            return PhysicalFile(filePath, "text/html");
        }
    }
}
