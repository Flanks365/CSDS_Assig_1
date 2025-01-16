using CSDS_Assign_1;
using Microsoft.Data.SqlClient;
using System.Data;

public class Repository : IRepository, IDisposable
{
    private readonly string databaseIP = "192.168.1.165"; 
    private readonly string myDatabase = "master";
    private readonly string myUser = "sa";
    private readonly string myPassword = "oracle12";


    private SqlConnection? con;
    public ResultSet rs;

    public Repository()
    {
        con = null;
        rs = new ResultSet();
    }

    public void Init(string db, string user, string password)
    {
        try
        {
            string connection = $"Data Source={databaseIP},1433;Initial Catalog={db};User ID={user};Password={password};TrustServerCertificate=true;";
            con = new SqlConnection(connection);
            con.Open();
            Console.WriteLine("Connection established.");
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
    }

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

    public void Insert(string tableString, string valueString)
    {
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
    }

    public void Insert(string tableString, string setString, string valueString)
    {
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
    }

    public void Insert(string tableString, string setString, string valueString, string type, Stream inputStream)
    {
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
    }

    public void Update(string tableString, string setString, string conditionString)
    {
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
    }

    public void Update(string tableString, string setString, Stream inputStream)
    {
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
    }

    public void Delete(string tableString, string conditionString)
    {
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
    }

    public void Select(string fieldString, string tableString, string conditionString)
    {
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
    }

    public void Select(string fieldString, string tableString)
    {
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
    }

    private static byte[] ReadStreamAsBytes(Stream stream)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }

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

    public void Dispose()
    {
        con?.Dispose();
    }

    public List<Category> GetCategories()
    {
        List<Category> categories = new List<Category>();
        Init(myDatabase, myUser, myPassword);
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

public class ResultSet
{
    public List<Row> Rows { get; set; } = new List<Row>();
}

public class Row
{
    public List<object> Columns { get; set; } = new List<object>();
}
