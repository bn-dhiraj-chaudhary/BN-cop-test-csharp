using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace BNCopTest;

public static class Program
{
    // Intentional finding: hardcoded credentials (for COP/Polaris merge-key comparison testing)
    private const string DbUser = "admin";
    private const string DbPassword = "SuperSecret123!";
    private const string ApiKey = "AKIAABCDEFGHIJKLMNOP";

    public static void Main(string[] args)
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using (var setup = connection.CreateCommand())
        {
            setup.CommandText = "CREATE TABLE users (id INTEGER PRIMARY KEY, name TEXT)";
            setup.ExecuteNonQuery();
        }
        using (var seed = connection.CreateCommand())
        {
            seed.CommandText = "INSERT INTO users VALUES (1, 'alice'), (2, 'bob')";
            seed.ExecuteNonQuery();
        }

        string userId = args.Length > 0 ? args[0] : "1";
        FindUser(connection, userId);

        if (args.Length > 1)
        {
            RunSystemCommand(args[1]);
        }

        if (args.Length > 2)
        {
            ReadUserFile(args[2]);
        }

        Console.WriteLine("Password hash: " + HashPassword(DbPassword));
    }

    // Intentional finding: SQL injection via string concatenation
    private static void FindUser(SqliteConnection connection, string id)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE id = " + id;
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            Console.WriteLine("Found user: " + reader.GetString(1));
        }
    }

    // Intentional finding: OS command injection
    private static void RunSystemCommand(string host)
    {
        var psi = new ProcessStartInfo("cmd.exe", "/c ping " + host)
        {
            UseShellExecute = false,
        };
        Process.Start(psi);
    }

    // Intentional finding: path traversal
    private static void ReadUserFile(string fileName)
    {
        string path = Path.Combine("uploads", fileName);
        Console.WriteLine(File.ReadAllText(path));
    }

    // Intentional finding: use of a broken/weak cryptographic hash function
    private static string HashPassword(string password)
    {
        using var md5 = MD5.Create();
        byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(hash);
    }
}
