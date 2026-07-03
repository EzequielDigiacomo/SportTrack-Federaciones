using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSaaSPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxAtletas", "Nombre", "Precio" },
                values: new object[] { 500, "SIGDEF (S)", 50m });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxAtletas", "Nombre", "Precio" },
                values: new object[] { 2000, "SIGDEF (M)", 120m });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "SIGDEF (L)");

            migrationBuilder.InsertData(
                schema: "catalogos",
                table: "PlanesSaaS",
                columns: new[] { "Id", "ExportacionExcel", "MaxAtletas", "MaxTorneosActivos", "Nombre", "Precio", "ResultadosTiempoReal", "SoportePrioritario" },
                values: new object[,]
                {
                    { 4, false, 500, 5, "SportTrack (S)", 40m, false, false },
                    { 5, false, 2000, 20, "SportTrack (M)", 90m, false, false },
                    { 6, true, -1, -1, "SportTrack (L)", 190m, true, true },
                    { 7, true, 500, 5, "Pack Dúo (S)", 75m, true, true },
                    { 8, true, 2000, 20, "Pack Dúo (M)", 170m, true, true },
                    { 9, true, -1, -1, "Pack Dúo (L)", 350m, true, true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "MaxAtletas", "Nombre", "Precio" },
                values: new object[] { 1000, "Bronce", 0m });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "MaxAtletas", "Nombre", "Precio" },
                values: new object[] { 4000, "Plata", 99m });

            migrationBuilder.UpdateData(
                schema: "catalogos",
                table: "PlanesSaaS",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nombre",
                value: "Oro");
        }
    }
}
