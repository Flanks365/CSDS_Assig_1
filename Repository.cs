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
    private readonly string databaseIP = "10.65.44.204";
    private readonly string databasePort = "1434"; // 1433 is default
    private readonly string myDatabase = "master";
    private readonly string myUser = "sa";
    private readonly string myPassword = "oracle";


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
                byte[] binaryData = ReadStreamAsBytes(inputStream);
                stmt.Parameters.Add(new SqlParameter(type.Equals("blob", StringComparison.OrdinalIgnoreCase) ? "@blobData" : "@binaryData", SqlDbType.VarBinary)
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
                    string categoryName = row.Columns[1].ToString().Trim();
                    string imgType = row.Columns[2].ToString().Trim();

                    byte[]? image = row.Columns[3] as byte[];

                    categories.Add(new Category(categoryName, imgType, image!));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while parsing categories: " + ex.Message);
        }
        return categories;
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
