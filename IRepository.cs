using System.IO;

public interface IRepository
{
    void Init(string connectString, string user, string password);
    void Close();
    void Insert(string tableString, string valueString);
    void Insert(string tableString, string setString, string valueString);
    void Insert(string tableString, string setString, string valueString, string type, Stream inputStream);
    void Update(string tableString, string setString, string conditionString);
    void Update(string tableString, string setString, Stream inputStream);
    void Delete(string tableString, string conditionString);
    void Select(string fieldString, string tableString, string conditionString);
    void Select(string fieldString, string tableString);
    byte[]? GetBlobAsBytes(string columnName);
}
