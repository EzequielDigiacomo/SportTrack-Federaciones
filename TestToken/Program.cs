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
            Console.WriteLine("ConexiÃ³n exitosa a Render.");

            // 1. Todos los planes: competencias ilimitadas
            using (var cmd = new NpgsqlCommand(@"UPDATE catalogos.""PlanesSaaS"" SET ""MaxTorneosActivos"" = -1;", conn))
            {
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"MaxTorneosActivos = -1 aplicado a {rows} planes.");
            }

            // 2. Planes M: activar ResultadosTiempoReal y ExportacionExcel
            using (var cmd = new NpgsqlCommand(@"
                UPDATE catalogos.""PlanesSaaS""
                SET ""ResultadosTiempoReal"" = true, ""ExportacionExcel"" = true
                WHERE ""Nombre"" LIKE '%(M)';", conn))
            {
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"ResultadosTiempoReal + Excel = true aplicado a {rows} planes M.");
            }

            // 3. Todos los planes: ExportacionExcel = true (todos tienen Excel)
            using (var cmd = new NpgsqlCommand(@"UPDATE catalogos.""PlanesSaaS"" SET ""ExportacionExcel"" = true;", conn))
            {
                int rows = cmd.ExecuteNonQuery();
                Console.WriteLine($"ExportacionExcel = true aplicado a {rows} planes.");
            }

            // 4. Verificar resultado final
            Console.WriteLine("\n--- Estado final de los planes ---");
            using (var cmd = new NpgsqlCommand(@"SELECT ""Id"", ""Nombre"", ""MaxTorneosActivos"", ""ResultadosTiempoReal"", ""ExportacionExcel"", ""SoportePrioritario"" FROM catalogos.""PlanesSaaS"" ORDER BY ""Id"";", conn))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Id | Nombre | MaxTorneos | TiempoReal | Excel | SoportePrioritario");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader[0]} | {reader[1]} | {reader[2]} | {reader[3]} | {reader[4]} | {reader[5]}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
