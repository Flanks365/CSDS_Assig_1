using CSDS_Assign_1;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;

/// <summary>
/// Provides methods for database operations, including CRUD operations and connection management.
/// Implements IDisposable to handle resource cleanup.
/// </summary>
public class Repository : IRepository, IDisposable
{
    // CHANGE THESE TO YOUR OWN
    private readonly string databaseIP = "127.0.0.1"; // database IP, try 192.168.1.165
    private readonly string databasePort = "1434"; // 1433 is default
    private readonly string myDatabase = "QuizApp";
    private readonly string myUser = "sa";
    private readonly string myPassword = "Oracle12!";

    //127.0.0.1
    //Oracle12!
    private SqlConnection? con;
    public ResultSet rs;

    /// <summary>
    /// Initializes a new instance of the Repository class.
    /// Sets up the connection object and result set container.
    /// </summary>
    public Repository()
    {
        con = null;
        rs = new ResultSet();
    }

    /// <summary>
    /// Initializes the database connection using predefined credentials.
    /// </summary>
    public void Init()
    {
        try
        {
            string connection = $"Data Source={databaseIP},{databasePort};Initial Catalog={myDatabase};User ID={myUser};Password={myPassword};TrustServerCertificate=true;";
            con = new SqlConnection(connection);
            con.Open();
            Console.WriteLine("Connection established.");
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
    }

    /// <summary>
    /// Initializes the database connection using custom credentials.
    /// </summary>
    /// <param name="db">Database name</param>
    /// <param name="user">Username</param>
    /// <param name="password">Password</param>
    public void Init(string db, string user, string password)
    {
        try
        {
            string connection = $"Data Source={databaseIP},{databasePort};Initial Catalog={db};User ID={user};Password={password};TrustServerCertificate=true;";
            con = new SqlConnection(connection);
            con.Open();
            Console.WriteLine("Connection established.");
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
    }

    /// <summary>
    /// Closes the database connection.
    /// </summary>
    public void Close()
    {
        try
        {
            con?.Close();
            Console.WriteLine("Connection closed.");
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
    }

    /// <summary>
    /// Inserts a row into a table with specified values.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="valueString">Values to insert</param>
    public void Insert(string tableString, string valueString)
    {
        Init();
        try
        {
            string query = $"INSERT INTO {tableString} VALUES ({valueString})";
            Console.WriteLine(query);
            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows inserted.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }

    /// <summary>
    /// Inserts a row into a table with specified columns and values.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="setString">Columns</param>
    /// <param name="valueString">Values</param>
    public void Insert(string tableString, string setString, string valueString)
    {
        Init();
        try
        {
            string query = $"INSERT INTO {tableString} ({setString}) VALUES ({valueString})";
            Console.WriteLine(query);
            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows inserted.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }

    /// <summary>
    /// Inserts a row into a table with binary data, such as blobs.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="setString">Columns</param>
    /// <param name="valueString">Values</param>
    /// <param name="type">Type of binary data (e.g., blob)</param>
    /// <param name="inputStream">Stream containing binary data</param>
    public void Insert(string tableString, string setString, string valueString, string type, Stream inputStream)
    {
        Init();
        try
        {
            string query = $"INSERT INTO {tableString} ({setString}) VALUES ({valueString})";
            Console.WriteLine(query);

            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                // Add binary data as a parameter
                byte[] binaryData = ReadStreamAsBytes(inputStream);
                stmt.Parameters.Add(new SqlParameter("@FileData", SqlDbType.VarBinary)
                {
                    Value = binaryData
                });

                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows inserted.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }


    /// <summary>
    /// Updates rows in a table based on a condition.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="setString">Update expression</param>
    /// <param name="conditionString">Condition for updating rows</param>
    public void Update(string tableString, string setString, string conditionString)
    {
        Init();
        try
        {
            string query = $"UPDATE {tableString} SET {setString} WHERE {conditionString}";
            Console.WriteLine(query);
            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows updated.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }

    /// <summary>
    /// Version of update that takes value and conditions
    /// </summary>
    /// <param name="tableName"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    /// <param name="condition"></param>
    public void Update(string tableName, string column, string value, string condition)
    {
        Init();
        try
        {
            // Construct the query with parameters
            string query = $"UPDATE {tableName} SET {column} = @Value WHERE {condition}";
            Console.WriteLine(query);

            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                // Add parameter to prevent SQL injection
                stmt.Parameters.AddWithValue("@Value", value);

                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows updated.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        finally
        {
            Close();
        }
    }



    /// <summary>
    /// Updates rows in a table based on a condition and uses parameters for safe execution.
    /// </summary>
    /// <param name="table">Table name</param>
    /// <param name="setString">Set expression (e.g., "column1 = @value1")</param>
    /// <param name="whereString">Where condition (e.g., "id = @id")</param>
    /// <param name="parameters">Object containing parameters to be used in the query</param>
    public void Update(string table, string setString, string whereString, object parameters)
    {
        Init();
        try
        {
            // Construct the SQL query with the table name, set string, and where condition
            string query = $"UPDATE {table} SET {setString} WHERE {whereString}";
            Console.WriteLine("Generated Query: " + query);

            using (SqlCommand command = new SqlCommand(query, con))
            {
                // Add parameters to the command object
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    var paramName = "@" + prop.Name;
                    var paramValue = prop.GetValue(parameters);
                    command.Parameters.AddWithValue(paramName, paramValue);

                    // Log the added parameter for debugging purposes
                    Console.WriteLine($"Parameter Added: {paramName} = {paramValue} (Type: {paramValue?.GetType()})");
                }

                // Execute the query
                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows updated.");
            }
        }
        catch (SqlException ex)
        {
            // Log detailed SQL exception information
            Console.WriteLine("SQL Exception occurred:");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"SQLState: {ex.State}");
            Console.WriteLine($"ErrorCode: {ex.ErrorCode}");

            // Optionally, log each inner exception (if any)
            SqlException innerEx = ex.InnerException as SqlException;
            while (innerEx != null)
            {
                Console.WriteLine("Inner SQL Exception:");
                Console.WriteLine($"Message: {innerEx.Message}");
                innerEx = innerEx.InnerException as SqlException;
            }
        }
        catch (Exception ex)
        {
            // Log other exceptions (non-SQL exceptions)
            Console.WriteLine("Exception occurred: " + ex.Message);
        }
        finally
        {
            Close();
        }
    }





    /// <summary>
    /// Updates rows in a table based on a binary data stream.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="setString">Update expression</param>
    /// <param name="inputStream">Stream containing binary data</param>
    public void Update(string tableString, string setString, Stream inputStream)
    {
        Init();
        try
        {
            string query = $"UPDATE {tableString} SET {setString}";
            Console.WriteLine(query);
            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                byte[] binaryData = ReadStreamAsBytes(inputStream);
                stmt.Parameters.Add(new SqlParameter("@binaryData", SqlDbType.VarBinary)
                {
                    Value = binaryData
                });

                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows updated.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }


    /// <summary>
    /// Deletes rows in a table based on a specified condition.
    /// </summary>
    /// <param name="tableString">Table name</param>
    /// <param name="conditionString">Condition for deleting rows</param>
    public void Delete(string tableString, string conditionString)
    {
        Init();
        try
        {
            string query = $"DELETE FROM {tableString} WHERE {conditionString}";
            Console.WriteLine(query);
            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows deleted.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }

    public void Delete(string tableString, string conditionString, int questionId)
    {
        Init();
        try
        {
            // Ensure parameterized query to avoid SQL injection
            string query = $"DELETE FROM {tableString} WHERE {conditionString}";
            Console.WriteLine(query);

            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                // Add parameter to the query
                stmt.Parameters.AddWithValue("@questionId", questionId);

                // Execute the query
                int rowsAffected = stmt.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows deleted.");
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }



    /// <summary>
    /// Selects rows from a table based on a specified condition.
    /// </summary>
    /// <param name="fieldString">Fields to select</param>
    /// <param name="tableString">Table name</param>
    /// <param name="conditionString">Condition for selecting rows</param>
    public void Select(string fieldString, string tableString, string conditionString)
    {
        Init();
        try
        {
            string query = $"SELECT {fieldString} FROM {tableString} WHERE {conditionString}";
            Console.WriteLine(query);

            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = stmt.ExecuteReader())
                {
                    rs.Rows.Clear();

                    while (reader.Read())
                    {
                        Row row = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetFieldType(i) == typeof(byte[]))
                            {
                                row.Columns.Add(reader.GetValue(i));
                            }
                            else if (reader.GetFieldType(i) == typeof(Int32))
                            {
                                row.Columns.Add(reader.GetInt32(i).ToString());
                            }
                            else if (reader.GetFieldType(i) == typeof(Guid))
                            {
                                row.Columns.Add(reader.GetGuid(i).ToString());
                            }
                            else
                            {
                                row.Columns.Add(reader.GetString(i));
                            }
                        }
                        rs.Rows.Add(row);
                    }
                    Console.WriteLine("Query result stored in rs.");
                }
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }

    /// <summary>
    /// Selects rows from a table without any condition.
    /// </summary>
    /// <param name="fieldString">Fields to select</param>
    /// <param name="tableString">Table name</param>
    public void Select(string fieldString, string tableString)
    {
        Init();
        try
        {
            string query = $"SELECT {fieldString} FROM {tableString}";
            Console.WriteLine(query);

            using (SqlCommand stmt = new SqlCommand(query, con))
            {
                using (SqlDataReader reader = stmt.ExecuteReader())
                {
                    rs.Rows.Clear();

                    while (reader.Read())
                    {
                        Row row = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            // Handle byte[] fields
                            if (reader.GetFieldType(i) == typeof(byte[]))
                            {
                                row.Columns.Add(reader.GetValue(i));
                            }
                            // Handle Guid fields
                            else if (reader.GetFieldType(i) == typeof(Guid))
                            {
                                row.Columns.Add(reader.GetGuid(i).ToString());
                            }
                            // Handle integer fields (e.g., 'id')
                            else if (reader.GetFieldType(i) == typeof(int))
                            {
                                row.Columns.Add(reader.GetInt32(i)); // Directly get the integer value
                            }
                            // Handle string fields
                            else if (reader.GetFieldType(i) == typeof(string))
                            {
                                row.Columns.Add(reader.GetString(i));
                            }
                            else
                            {
                                row.Columns.Add(reader.GetValue(i)); // Default case for other types
                            }
                        }
                        rs.Rows.Add(row);
                    }
                    Console.WriteLine("Query result stored in rs.");
                }
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        Close();
    }


    /// <summary>
    /// Reads a binary stream and converts it into a byte array.
    /// </summary>
    /// <param name="stream">The input stream</param>
    /// <returns>Byte array of the stream's data</returns>
    private static byte[] ReadStreamAsBytes(Stream stream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Logs details of a caught SqlException.
    /// </summary>
    /// <param name="ex">The SqlException to log</param>
    private static void PrintSqlException(SqlException? ex)
    {
        while (ex != null)
        {
            Console.WriteLine("--- SQLException caught ---");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"SQLState: {ex.State}");
            Console.WriteLine($"ErrorCode: {ex.ErrorCode}");
            ex = ex.InnerException as SqlException;
        }
    }

    /// <summary>
    /// Releases the resources used by the repository.
    /// </summary>
    public void Dispose()
    {
        con?.Dispose();
    }

    /// <summary>
    /// Retrieves a list of categories from the "categories" table.
    /// </summary>
    /// <returns>List of categories</returns>
    public List<Category> GetCategories()
    {
        List<Category> categories = new List<Category>();

        Select("*", "categories");

        try
        {
            foreach (var row in rs.Rows)
            {
                if (row.Columns.Count >= 3)
                {
                    // Directly cast the 'id' column to int (as it's an int in the database)
                    int id = Convert.ToInt32(row.Columns[0]);
                    string categoryName = row.Columns[1].ToString().Trim();
                    string imgType = row.Columns[2].ToString().Trim();
                    byte[]? image = row.Columns[3] as byte[];
                    // Create a new Category with the int id
                    categories.Add(new Category(id, categoryName, imgType, image!));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while parsing categories: " + ex.Message);
        }
        return categories;
    }


    public List<Question> GetQuestionsNoAnswers(string categoryId)
    {
        List<Question> questions = new List<Question>();
        Select("*", "questions", $"category_id = {categoryId}");
        try
        {

            foreach (var row in rs.Rows)
            {
                if (row.Columns.Count >= 6)
                {
                    string id = row.Columns[0].ToString().Trim();
                    string text = row.Columns[1].ToString().Trim();
                    string mediaType = row.Columns[2].ToString().Trim();
                    byte[]? mediaContent = row.Columns[3] as byte[];
                    string mediaPreview = row.Columns[4].ToString().Trim();
                    //string categoryId = row.Columns[5].ToString().Trim();
                    questions.Add(new Question(id, text, mediaType, mediaContent!, mediaPreview, categoryId));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while parsing categories: " + ex.Message);
        }
        return questions;
    }


    public List<Questions_W_Answers> GetQuestions(string categoryId)
    {
        List<Question> questions = new List<Question>();
        List<Answer> answers = new List<Answer>();
        List<Questions_W_Answers> res = new List<Questions_W_Answers>();

        Console.WriteLine($"Fetching questions for categoryId: {categoryId}");

        Select("*", "questions", $"category_id = {categoryId}");

        try
        {
            // Printing number of rows fetched from the database
            Console.WriteLine($"Rows fetched: {rs.Rows.Count}");

            foreach (var row in rs.Rows)
            {
                if (row.Columns.Count >= 6)
                {
                    string id = row.Columns[0].ToString().Trim();
                    string text = row.Columns[1].ToString().Trim();
                    string mediaType = row.Columns[2].ToString().Trim();
                    byte[]? mediaContent = row.Columns[3] as byte[];
                    string mediaPreview = row.Columns[4].ToString().Trim();
                    //string categoryId = row.Columns[5].ToString().Trim();

                    Console.WriteLine($"Question Id: {id}, Text: {text}, MediaType: {mediaType}, MediaPreview: {mediaPreview}");

                    questions.Add(new Question(id, text, mediaType, mediaContent!, mediaPreview, categoryId));
                }
            }

            foreach (var question in questions)
            {
                Console.WriteLine($"Fetching answers for questionId: {question.Id}");
                answers = GetAnswers(question.Id);

                List<String> answersList = new List<String>();
                var correctAnswer = answers.FirstOrDefault(a => a.IsCorrect == "Y");
                if (correctAnswer != null)
                {
                    answersList.Insert(0, correctAnswer.AnswerText); // Add correct answer first
                }

                // Add other answers, avoiding duplicates
                foreach (var ans in answers)
                {
                    if (ans.IsCorrect != "Y" && !answersList.Contains(ans.AnswerText))
                    {
                        answersList.Add(ans.AnswerText);
                    }

                    // Ensure no more than 4 answers are added
                    if (answersList.Count == 4)
                    {
                        break;
                    }
                }

                // Log answers
                Console.WriteLine($"Answers for question {question.Id}: {string.Join(", ", answersList)}");

                res.Add(new Questions_W_Answers(question.Id,
                    answersList.ElementAtOrDefault(0), answersList.ElementAtOrDefault(1),
                    answersList.ElementAtOrDefault(2), answersList.ElementAtOrDefault(3),
                    question.MediaType, question.MediaPreview, question.Text));
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while parsing categories: " + ex.Message);
        }
        return res;
    }

    public List<Answer> GetAnswers(string questionId)
    {
        List<Answer> answers = new List<Answer>();

        Console.WriteLine($"Fetching answers for questionId: {questionId}");
        Select("*", "answers", $"question_id = {questionId}");

        try
        {
            // Printing number of rows fetched for answers
            Console.WriteLine($"Rows fetched for answers: {rs.Rows.Count}");

            foreach (var row in rs.Rows)
            {
                if (row.Columns.Count >= 5)
                {
                    string id = row.Columns[0].ToString().Trim();
                    string answerText = row.Columns[2].ToString().Trim();
                    string isCorrect = row.Columns[3].ToString().Trim();
                    string answerIndex = row.Columns[4].ToString().Trim();

                    Console.WriteLine($"Answer Id: {id}, AnswerText: {answerText}, IsCorrect: {isCorrect}, AnswerIndex: {answerIndex}");

                    answers.Add(new Answer(id, questionId, answerText, isCorrect, answerIndex));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while parsing answers: " + ex.Message);
        }
        return answers;
    }

    public User GetUser(int userId)
    {
        User user = null;
        Select("*", "users WHERE id = " + userId); // Assuming the 'Select' method allows SQL queries.
        try
        {
            foreach (var row in rs.Rows)
            {
                if (row.Columns.Count >= 4) // Ensure we have all 4 columns for the user (id, username, password, role)
                {
                    int id = Convert.ToInt32(row.Columns[0]);
                    string username = row.Columns[1].ToString().Trim();
                    string password = row.Columns[2].ToString().Trim();
                    string role = row.Columns[3].ToString().Trim();
                    // Create a new User object and return it
                    user = new User(id, username, password, role);
                    break; // Exit the loop after we find the user
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error (or handle it in a more robust way, e.g., logging)
            Console.WriteLine("Error while retrieving user: " + ex.Message);
        }
        return user;
    }
}

/// <summary>
/// Represents the result set of a database query, containing a collection of rows.
/// </summary>
public class ResultSet
{
    public List<Row> Rows { get; set; } = new List<Row>();
}

/// <summary>
/// Represents a single row in a database query result, containing a collection of column values.
/// </summary>
public class Row
{
    public List<object> Columns { get; set; } = new List<object>();
}