using CSDS_Assign_1;
using Microsoft.Data.SqlClient;
using System.Data;

/// <summary>
/// Provides methods for database operations, including CRUD operations and connection management.
/// Implements IDisposable to handle resource cleanup.
/// </summary>
public class Repository : IRepository, IDisposable
{
    // CHANGE THESE TO YOUR OWN
    private readonly string databaseIP = "127.0.0.1";
    private readonly string databasePort = "1433"; // 1433 is default
    private readonly string myDatabase = "master";
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


    public List<Question> GetQuestions(string categoryId)
    {
        List<Question> questions = new List<Question>();

        Select("*", "questions", $"categoryId = {categoryId}");

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
