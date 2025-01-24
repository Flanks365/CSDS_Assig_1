using System.ComponentModel.DataAnnotations;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace ControllerExample.Controllers
{
    [ApiController]
    public class LoginController : ControllerBase
    {
        Repository repository = new Repository();

        [HttpPost("loginPost")]
        /** param loginRequest represents an object that holds strings that user
         *  typed in the login form */
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            Console.WriteLine("in login post");

            //get username, password and role from db
            repository.Select("username, password, role", "users", $"username='{loginRequest.Username}'");

            //need error handing if they enter invalid username
            //fix bug where it displays login username on mainpage twice when logging in as admin

            string username = "";
            string dbPassword = "";
            string role = "";

            foreach (var row in repository.rs.Rows)
            {
                // Since we only selected one column (username), it will be in the first index (Columns[0])
                username = row.Columns[0].ToString().Trim();
                dbPassword = row.Columns[1].ToString().Trim(); //this should be the hashed password from the database
                role = row.Columns[2].ToString().Trim();
                Console.WriteLine($"Username: {username}");
            }

            // Check credentials (example only, use a secure approach in production)
            if (loginRequest.Username == username && BCrypt.Net.BCrypt.Verify(loginRequest.Password, dbPassword))
            {
                //string mainPage = getHTMLAsString("login.html");
                //mainPage = mainPage.Replace("[USER_ID]", username);
                HttpContext.Session.SetString("USERNAME", loginRequest.Username);
                
                if (role.Equals("admin"))
                {
                    HttpContext.Session.SetString("ROLE", "admin");
                    //mainPage = mainPage.Replace("<!--admin-->", getHTMLAsString("admin.html"));
                } else
                {
                    HttpContext.Session.SetString("ROLE", "user");
                }
                Console.WriteLine("login success!!!");
                return Ok();
            }
            else
            {
                Console.WriteLine("login failure!!!");
                return StatusCode(StatusCodes.Status405MethodNotAllowed);
            }
        }
    }

}
