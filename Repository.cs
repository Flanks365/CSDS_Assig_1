using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.IO;
using CSDS_Assign_1;

public class Repository : IRepository, IDisposable
{
    private readonly string databaseIP = "192.168.1.165"; // Replace this with your database IP
    private SqlConnection? con;
    public String? rs;

    public Repository()
    {
        con = null;
        rs = null;
    }

    public void Init(string connectString, string user, string password)

    {
        try
        {
            string connection = $"Data Source={databaseIP},1433;Initial Catalog=XE;User ID={user};Password={password}";
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
                    // Initialize rs as an empty string
                    rs = string.Empty;

                    while (reader.Read())
                    {
                        // Example: Append all columns of the current row to rs
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rs += reader[i].ToString() + "\t";  // Tab separated
                        }
                        rs += "\n";  // New line for each row
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
                    // Initialize rs as an empty string
                    rs = string.Empty;

                    while (reader.Read())
                    {
                        // Example: Append all columns of the current row to rs
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            rs += reader[i].ToString() + "\t";  // Tab separated
                        }
                        rs += "\n";  // New line for each row
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

    public byte[]? GetBlobAsBytes(string columnName)
    {
        try
        {
            using (SqlDataReader reader = con!.CreateCommand().ExecuteReader())
            {
                if (reader.HasRows)
                {
                    int columnIndex = reader.GetOrdinal(columnName);
                    if (!reader.IsDBNull(columnIndex))
                    {
                        return (byte[])reader[columnIndex];
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            PrintSqlException(ex);
        }
        return null;
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

        try
        {
            // Ensure that rs has been populated by calling Select earlier
            if (string.IsNullOrEmpty(rs))
            {
                Console.WriteLine("No data to parse. Ensure Select() is called first.");
                return categories;
            }

            // Split the results by rows (new lines)
            string[] rows = rs.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string row in rows)
            {
                // Split each row by tab characters to get individual fields
                string[] columns = row.Split('\t');

                if (columns.Length >= 3)
                {
                    string categoryName = columns[0].Trim();
                    string imgType = columns[2].Trim();

                    // Assuming you have a method to retrieve the binary data (image)
                    byte[]? image = GetBlobAsBytes("image"); 

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
