using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        
        [HttpGet("getQuestions")]
        public IActionResult GetQuestions([FromQuery] int id, [FromQuery] string quizName)
        {
            try
            {
                // Query the database to get the questions for the given quizId
                var questions = _repository.GetQuestions(id.ToString());
                // Adjust with your actual repository and method
                foreach (var item in questions)
                {
                    Console.WriteLine("Question with Answers details:");
                    Console.WriteLine($"QuestId: {item.QuestId}");
                    Console.WriteLine($"Corr: {item.Corr}");
                    Console.WriteLine($"Dec1: {item.Dec1}");
                    Console.WriteLine($"Dec2: {item.Dec2}");
                    Console.WriteLine($"Dec3: {item.Dec3}");
                    Console.WriteLine($"MediaTyp: {item.MediaTyp}");
                    Console.WriteLine($"MediaPrev: {item.MediaPrev}");
                    Console.WriteLine($"Question: {item.Question}");
                    Console.WriteLine("--------------------------------------------------");
                }
                
                
                if (questions == null || !questions.Any())
                {
                    return NotFound("No questions found.");
                }
                

                // Return the data as JSON
                var json = JsonConvert.SerializeObject(questions);
                return Ok(json);
            }
            catch (Exception ex)
            {
                // Handle any errors
                Console.WriteLine("Error fetching questions: " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("postQuestions")]
        public IActionResult PostQuestions()
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
                
                var question = formData["Question"];
                var answer = formData["Answer"];
                var decoy1 = formData["Decoy1"];
                var decoy2 = formData["Decoy2"];
                var decoy3 = formData["Decoy3"];
                var contentType = formData["ContentType"];
                var fileName = formData["FileName"];
                var quoteText = formData["QuoteText"];
                var quizId = int.Parse(formData["id"].ToString());
                
                byte[] fileBytes = null;
                string imageType = null;
                
                if (contentType == "image")
                {
                    var file = Request.Form.Files.GetFile("FileName");
                    imageType = file.ContentType;
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }
                
                // Construct the SQL insert query
                string tableString = "questions"; // Table name
                string setString = "question_text, media_type, media_content, media_preview, category_id";
                string valueString = $"'{question}', '{contentType}', @FileData, '{quoteText}', {quizId}";
                
                _repository.Insert(tableString, setString, valueString, "blob", new MemoryStream(fileBytes));
                
                // Construct answers SQL insert query
                string answersTableString = "answers";
                string answersSetString = "question_id, answer_text, is_correct, answer_index";  // Removed the semicolon here
                string answersValueString = $"(SELECT MAX(id) FROM questions), '{answer.ToString()}', 'Y', 1";
                string decoy1ValueString = $"(SELECT MAX(id) FROM questions), '{decoy1.ToString()}', 'N', 2";
                string decoy2ValueString = $"(SELECT MAX(id) FROM questions), '{decoy2.ToString()}', 'N', 3";
                string decoy3ValueString = $"(SELECT MAX(id) FROM questions), '{decoy3.ToString()}', 'N', 4";

                
                _repository.Insert(answersTableString, answersSetString, answersValueString);
                _repository.Insert(answersTableString, answersSetString, decoy1ValueString);
                _repository.Insert(answersTableString, answersSetString, decoy2ValueString);
                _repository.Insert(answersTableString, answersSetString, decoy3ValueString);
                
                return Ok("Question added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("updateQuest")]
        public IActionResult UpdateQuestions()
        {
            try
            {
                var formData = Request.Form;

                var questionId = int.Parse(formData["questionId"].ToString());

                var questionText = formData["Question"].ToString();
                var ansText = formData["Answer"].ToString();
                var dec1 = formData["Decoy1"].ToString();
                var dec2 = formData["Decoy2"].ToString();
                var dec3 = formData["Decoy3"].ToString();

                var contentType = formData["ContentType"];
                var quoteText = formData["QuoteText"];

                byte[] fileBytes = null;
                string imageType = null;

                
                var file = Request.Form.Files.GetFile("FileName");
                imageType = file.ContentType;
                if (file != null && file.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        file.CopyTo(memoryStream);
                        fileBytes = memoryStream.ToArray();
                    }
                }


                string tableString = "questions";
                string setString = "question_text, media_type, media_content, media_preview";
                string valueString = $"{questionText}, {contentType}, {quoteText}";
                string condString = $"id = {questionId}";
                _repository.Update(tableString, setString, valueString, condString);

                if (fileBytes != null)
                {
                    string imageSet = "media_content = @binaryData";
                    _repository.Update(tableString, imageSet, new MemoryStream(fileBytes));
                }

                string answersTableString = "answers";
                string answer1setString = "answer_text";
                string answer2setString = "answer_text";
                string answer3setString = "answer_text";
                string answer4setString = "answer_text";

                string answer1Value = $"{ansText}";
                string answer2Value = $"{dec1}";
                string answer3Value = $"{dec2}";
                string answer4Value = $"{dec3}";

                string answer1Cond = $"{questionId}, {1}";
                string answer2Cond = $"{questionId}, {2}";
                string answer3Cond = $"{questionId}, {3}";
                string answer4Cond = $"{questionId}, {4}";

                _repository.Update(answersTableString, answer1setString, answer1Value, answer1Cond);
                _repository.Update(answersTableString, answer2setString, answer2Value, answer2Cond);
                _repository.Update(answersTableString, answer3setString, answer3Value, answer3Cond);
                _repository.Update(answersTableString, answer4setString, answer4Value, answer4Cond);

                return Ok("Question updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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


