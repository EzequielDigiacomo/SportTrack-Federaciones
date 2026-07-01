using System;
using Npgsql;

class Program
{
    static void Main()
    {
        string connString = "Host=dpg-d92j1qtaeets73em115g-a.virginia-postgres.render.com;Port=5432;Database=sporttrack_federaciones_db;Username=sporttrack_federaciones_db_user;Password=PhV0bbcKZELBn4dGE5lU66hIhQfUIDd4;SSL Mode=Require;Trust Server Certificate=true";
        try
        {
            using var conn = new NpgsqlConnection(connString);
            conn.Open();
            Console.WriteLine("Conexión exitosa.");

            using var cmd = new NpgsqlCommand("SELECT \"Username\", \"Rol\", \"Activo\" FROM seguridad.\"Usuarios\";", conn);
            using var reader = cmd.ExecuteReader();
            Console.WriteLine("Usuarios en la base de datos:");
            while (reader.Read())
            {
                Console.WriteLine($"- User: {reader.GetString(0)}, Role: {reader.GetString(1)}, Active: {reader.GetBoolean(2)}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
