using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;

namespace CSDS_Assign_1.Controllers
{
    /// <summary>
    /// Controller that handles requests for serving HTML pages and retrieving data.
    /// </summary>
    public class MyController : ControllerBase
    {
        private readonly Repository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyController"/> class.
        /// </summary>
        /// <param name="repository">The repository instance for database operations.</param>
        public MyController(Repository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Serves the Login HTML page.
        /// </summary>
        /// <returns>An IActionResult containing the Login HTML file.</returns>
        [HttpGet("login")]
        public IActionResult Login()
        {
            return GetHtmlFile("login");
        }

        /// <summary>
        /// Serves the Signup HTML page.
        /// </summary>
        /// <returns>An IActionResult containing the Signup HTML file.</returns>
        [HttpGet("signup")]
        public IActionResult Signup()
        {
            return GetHtmlFile("signup");
        }

        /// <summary>
        /// Serves the Main HTML page.
        /// </summary>
        /// <returns>An IActionResult containing the Main HTML file.</returns>
        [HttpGet("main")]
        public IActionResult Main()
        {
            return GetHtmlFile("main");
        }

        /// <summary>
        /// Serves the Categories HTML page.
        /// </summary>
        /// <returns>An IActionResult containing the Categories HTML file.</returns>
        [HttpGet("categories")]
        public IActionResult Categories()
        {
            return GetHtmlFile("categories");
        }

        /// <summary>
        /// Serves the QuizPage HTML file and processes query parameters.
        /// </summary>
        /// <param name="category_name">The category name to filter questions (optional).</param>
        /// <param name="autoplay">A flag to enable or disable autoplay (optional).</param>
        /// <returns>An IActionResult containing the QuizPage HTML file.</returns>
        [HttpGet("QuizPage")]
        public IActionResult QuizPage(string? category_name = null, bool? autoplay = null)
        {
            // Log the query parameters for debugging purposes
            Console.WriteLine($"Category Name: {category_name}");
            Console.WriteLine($"Autoplay: {autoplay}");

            // Add logic to handle query parameters if needed
            // For example, pass these values to the frontend via JavaScript or process them server-side

            return GetHtmlFile("quizPage");
        }

        /// <summary>
        /// Retrieves the list of categories from the database.
        /// </summary>
        /// <returns>An IActionResult containing the list of categories or an error message.</returns>
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

        /// <summary>
        /// Serves an HTML file from the specified path.
        /// </summary>
        /// <param name="str">The name of the HTML file (without extension).</param>
        /// <returns>An IActionResult containing the HTML file or a 404 error if not found.</returns>
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
