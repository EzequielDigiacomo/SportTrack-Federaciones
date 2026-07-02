using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminToSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Rol",
                value: "SuperAdmin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "Rol",
                value: "Admin");
        }
    }
}
