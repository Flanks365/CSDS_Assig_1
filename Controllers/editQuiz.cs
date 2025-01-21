using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;

namespace CSDS_Assign_1.Controllers
{
    
    public class EditQuiz_func : ControllerBase
    {
        private readonly Repository _repository;

        public EditQuiz_func(Repository repository)
        {
            _repository = repository;
            
        }

        [HttpPut("editCategories")]
        public IActionResult UpdateQuestions()
        {
            try
            {
                var formData = Request.Form;

                // Log the form data for debugging purposes
                foreach (var key in Request.Form.Keys)
                {
                    var value = Request.Form[key];
                    Console.WriteLine($"{key}: {value} (Type: {value.GetType()})");
                }

        
                // Extract the quizId and newQuizName from the form
                var quizId = int.Parse(formData["id"].ToString());
                
                var newQuizName = formData["newQuizName"].ToString();
        
                // Extract the uploaded file
                var file = formData.Files.GetFile("FileName");

                byte[] fileBytes = null;
                string imageType = null;

                if (file != null)
                {
                    imageType = file.ContentType;
            
                    // Read the file as byte array
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }

                // Construct the SQL update query
                string tableString = "categories"; // Table name
                string setString = "category_name = @newQuizName, image = @FileData, image_type = @imageType"; // Columns to update
                string whereString = $"id = @quizId"; // Condition for which record to update
        
                // Assuming Repository has an Update method that can handle this (adjust the method signature as necessary)
                _repository.Update(tableString, setString, whereString, 
                    new { newQuizName, FileData = fileBytes, imageType, quizId });

                return Ok(new { message = "Update successful" });

            }
            catch (Exception e)
            {
                Console.WriteLine("failed");
                return StatusCode(500, e.Message);
            }
        }



        [HttpDelete("deleteQuestions")]
        public IActionResult DeleteQuestions()
        {
            try
            {
                var formData = Request.Form;
                var questionId = int.Parse(formData["id"].ToString());
                string tableString = "categories";
                string whereString = $"id = {questionId}";
                _repository.Delete(tableString, whereString);
                return Ok(new { message = "Delete successful" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
             
            }
        }

        [HttpGet("getQuizzes")]
        public IActionResult getQuizzes()
        {
            try
            {
                // Get quizzes from repository
                List<Category> result = _repository.GetCategories();

                // Ensure the result is properly serialized
                if (result != null && result.Count > 0)
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(result);
                    
                    return Ok(json);
                }

                return NotFound("No quizzes found.");
            }
            catch (InvalidCastException ex)
            {
                // Log specific error
                Console.WriteLine($"Data type mismatch: {ex.Message}");
                return StatusCode(500, "Data type mismatch. Please check the database schema.");
            }
            catch (Exception ex)
            {
                // Log general error
                Console.WriteLine($"Error occurred: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred while retrieving quizzes.");
            }
        }


        [HttpPost("postQuizzes")]
        public IActionResult PostQuestion()
        {
            try
            {
                var formData = Request.Form;
                
                string QuizTitle = formData["QuizName"];
                var file = Request.Form.Files.GetFile("FileName");
                
             
                byte[] fileBytes = null;
                string imageType = null;

                if (file != null)
                {

                    imageType = file.ContentType;
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }
                
                string tableString = "categories"; // Table name
                string setString = "category_name, image, image_type";  // Columns in the table
                string valueString = $"'{QuizTitle}', @FileData, '{imageType}'"; // Include imageType in the values

                _repository.Insert(tableString, setString, valueString, "blob", new MemoryStream(fileBytes));

                
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return StatusCode(200, "no error");
        }

        [HttpPost("editQuizzes")]
        public IActionResult PostQuiz([FromForm] string title, [FromForm] string description)
        {
            return StatusCode(500);
        }
        
        
        
        //serve editQuiz HTML page
        [HttpGet("editQuiz")]
        public IActionResult EditQuiz()
        {
                return GetHtmlFile("editQuiz");
            
        }

        [HttpGet("editQuizzes")]
        public IActionResult EditQuizzes()
        {
            return GetHtmlFile("editQuizzes");
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


