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
        /// Handles user signup by accepting form data, hashing the password, and storing the user.
        /// </summary>
        /// <param name="signupModel">An object containing user signup data.</param>
        /// <returns>An IActionResult indicating success or failure.</returns>
        [HttpPost("signup")]
        public IActionResult Signup([FromBody] SignupModel signupModel)
        {
            if (signupModel == null || string.IsNullOrWhiteSpace(signupModel.Username) || string.IsNullOrWhiteSpace(signupModel.Password))
            {
                return BadRequest(new { message = "Invalid signup data." });
            }

            try
            {
                // Check if the username already exists
                var usernameCheckQuery = $"username = '{signupModel.Username}'";
                _repository.Select("*", "users", usernameCheckQuery);

                // Check if the query returned any results
                if (_repository.rs.Rows.Count > 0)
                {
                    return BadRequest(new { message = "Username already exists." });
                }

                // Hash the password using BCrypt
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signupModel.Password);


                // Save the user to the database (replace with your database logic)
                _repository.Insert("users", $"'{signupModel.Username}','{hashedPassword}','user'");

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the error (replace with your logging logic)
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        /// <summary>
        /// Serves the Main HTML page.
        /// </summary>
        /// <returns>An IActionResult containing the Main HTML file.</returns>
        [HttpGet("main")]
        public IActionResult Main()
        {
            var username = HttpContext.Session.GetString("USERNAME");
            string role = HttpContext.Session.GetString("ROLE");
            
            string mainPage = getHTMLAsString("main.html");
            
            if (role.Equals("admin"))
            {
                Console.WriteLine(getHTMLAsString("admin.html"));
                mainPage = mainPage.Replace("<!--admin-->", getHTMLAsString("admin.html"));
            }
            mainPage = mainPage.Replace("[USER_ID]", username);
            
            return Content(mainPage, "text/html");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            // Clear the entire session
            HttpContext.Session.Clear();
            Console.WriteLine("Session storage cleared");
            return GetHtmlFile("login");
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
        public IActionResult QuizPage(string? category_name = null, string? category_id = null, bool? autoplay = null)
        {
            // Log the query parameters for debugging purposes
            Console.WriteLine($"Category Name: {category_name}");
            Console.WriteLine($"Category ID: {category_id}");
            Console.WriteLine($"Autoplay: {autoplay}");

            // Add logic to handle query parameters if needed
            // For example, pass these values to the frontend via JavaScript or process them server-side

            //var questions = _repository.GetQuestions(category_id);
            //Console.WriteLine(questions);


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
        /// Retrieves the list of questions from the database, for the specified category.
        /// </summary>
        /// <returns>An IActionResult containing the list of questions or an error message.</returns>
        [HttpGet("questionsFromDB")]
        public IActionResult GetQuestions(string? category_id = null)
        {
            Console.WriteLine($"Category ID: {category_id}");
            try
            {
                var questions = _repository.GetQuestions(category_id);
                return Ok(questions); // Return 200 OK with the categories list
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves the list of answers from the database, for the specified question.
        /// </summary>
        /// <returns>An IActionResult containing the list of answers or an error message.</returns>
        [HttpGet("answersFromDB")]
        public IActionResult GetAnswers(string? question_id = null)
        {
            try
            {
                var questions = _repository.GetAnswers(question_id);
                return Ok(questions); // Return 200 OK with the categories list
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

        /// <summary>
        /// Retrieves a user from the database by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>An IActionResult containing the user data or an error message.</returns>
        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            try
            {
                // Assuming _repository.GetUser(id) retrieves a user by ID from the database
                var user = _repository.GetUser(id);

                if (user == null)
                {
                    return NotFound($"User with ID {id} not found."); // Return 404 if user is not found
                }

                return Ok(user); // Return 200 OK with the user data
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}"); // Return 500 if an error occurs
            }
        }


        /** Used to simplify getting the html page as a string. 
         *  Mainly for formatting parts of the html page.*/
        private static string getHTMLAsString(string htmlFile)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/HTML", htmlFile);
            // Check if the file exists

            if (!System.IO.File.Exists(filePath))
            {
                return null;
            }

            // Read and return the file content
            var htmlContent = System.IO.File.ReadAllText(filePath);

            return htmlContent;
        }
    }
}
