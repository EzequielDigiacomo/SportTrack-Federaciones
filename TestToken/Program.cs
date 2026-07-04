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
            Console.WriteLine("Conexión exitosa a Render.");

            using (var cmd = new NpgsqlCommand("SELECT \"Id\", \"Nombre\", \"FechaAltaPlan\", \"FechaVencimientoPlan\", \"Cuit\" FROM federacion.\"Federaciones\";", conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var nombre = reader.GetString(1);
                    var alta = reader.IsDBNull(2) ? "null" : reader.GetDateTime(2).ToString("yyyy-MM-dd");
                    var vence = reader.IsDBNull(3) ? "null" : reader.GetDateTime(3).ToString("yyyy-MM-dd");
                    var cuit = reader.IsDBNull(4) ? "null" : reader.GetString(4);
                    Console.WriteLine($"- Id: {id}, Nombre: {nombre}, Alta: {alta}, Vence: {vence}, Cuit: {cuit}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
