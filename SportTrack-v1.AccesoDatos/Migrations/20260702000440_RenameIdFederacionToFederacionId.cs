using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdFederacionToFederacionId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AtletasFederados_Federaciones_IdFederacion",
                schema: "federacion",
                table: "AtletasFederados");

            migrationBuilder.DropForeignKey(
                name: "FK_Clubes_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegadosClub_Federaciones_IdFederacion",
                schema: "federacion",
                table: "DelegadosClub");

            migrationBuilder.DropForeignKey(
                name: "FK_Entrenadores_Federaciones_IdFederacion",
                schema: "federacion",
                table: "Entrenadores");

            migrationBuilder.RenameColumn(
                name: "IdFederacion",
                schema: "federacion",
                table: "Federaciones",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "IdFederacion",
                schema: "federacion",
                table: "Entrenadores",
                newName: "FederacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Entrenadores_IdFederacion",
                schema: "federacion",
                table: "Entrenadores",
                newName: "IX_Entrenadores_FederacionId");

            migrationBuilder.RenameColumn(
                name: "IdFederacion",
                schema: "federacion",
                table: "DelegadosClub",
                newName: "FederacionId");

            migrationBuilder.RenameIndex(
                name: "IX_DelegadosClub_IdFederacion",
                schema: "federacion",
                table: "DelegadosClub",
                newName: "IX_DelegadosClub_FederacionId");

            migrationBuilder.RenameColumn(
                name: "ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                newName: "FederacionId");

            migrationBuilder.RenameIndex(
                name: "IX_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                newName: "IX_Clubes_FederacionId");

            migrationBuilder.RenameColumn(
                name: "IdFederacion",
                schema: "federacion",
                table: "AtletasFederados",
                newName: "FederacionId");

            migrationBuilder.RenameIndex(
                name: "IX_AtletasFederados_IdFederacion",
                schema: "federacion",
                table: "AtletasFederados",
                newName: "IX_AtletasFederados_FederacionId");

            migrationBuilder.AddColumn<int>(
                name: "FederacionId",
                schema: "seguridad",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                schema: "federacion",
                table: "Federaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "BloqueadaPorFaltaDePago",
                schema: "federacion",
                table: "Federaciones",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAltaPlan",
                schema: "federacion",
                table: "Federaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimientoPlan",
                schema: "federacion",
                table: "Federaciones",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlanSaaSId",
                schema: "federacion",
                table: "Federaciones",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sigla",
                schema: "federacion",
                table: "Federaciones",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FederacionId",
                schema: "regatas",
                table: "Eventos",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "FederacionId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_FederacionId",
                schema: "seguridad",
                table: "Usuarios",
                column: "FederacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Federaciones_PlanSaaSId",
                schema: "federacion",
                table: "Federaciones",
                column: "PlanSaaSId");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_FederacionId",
                schema: "regatas",
                table: "Eventos",
                column: "FederacionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AtletasFederados_Federaciones_FederacionId",
                schema: "federacion",
                table: "AtletasFederados",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Clubes_Federaciones_FederacionId",
                schema: "catalogos",
                table: "Clubes",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegadosClub_Federaciones_FederacionId",
                schema: "federacion",
                table: "DelegadosClub",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Entrenadores_Federaciones_FederacionId",
                schema: "federacion",
                table: "Entrenadores",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Federaciones_FederacionId",
                schema: "regatas",
                table: "Eventos",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Federaciones_PlanesSaaS_PlanSaaSId",
                schema: "federacion",
                table: "Federaciones",
                column: "PlanSaaSId",
                principalSchema: "catalogos",
                principalTable: "PlanesSaaS",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Federaciones_FederacionId",
                schema: "seguridad",
                table: "Usuarios",
                column: "FederacionId",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AtletasFederados_Federaciones_FederacionId",
                schema: "federacion",
                table: "AtletasFederados");

            migrationBuilder.DropForeignKey(
                name: "FK_Clubes_Federaciones_FederacionId",
                schema: "catalogos",
                table: "Clubes");

            migrationBuilder.DropForeignKey(
                name: "FK_DelegadosClub_Federaciones_FederacionId",
                schema: "federacion",
                table: "DelegadosClub");

            migrationBuilder.DropForeignKey(
                name: "FK_Entrenadores_Federaciones_FederacionId",
                schema: "federacion",
                table: "Entrenadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Federaciones_FederacionId",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Federaciones_PlanesSaaS_PlanSaaSId",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Federaciones_FederacionId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_FederacionId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Federaciones_PlanSaaSId",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_FederacionId",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "FederacionId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Activo",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "BloqueadaPorFaltaDePago",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "FechaAltaPlan",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "FechaVencimientoPlan",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "PlanSaaSId",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "Sigla",
                schema: "federacion",
                table: "Federaciones");

            migrationBuilder.DropColumn(
                name: "FederacionId",
                schema: "regatas",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "federacion",
                table: "Federaciones",
                newName: "IdFederacion");

            migrationBuilder.RenameColumn(
                name: "FederacionId",
                schema: "federacion",
                table: "Entrenadores",
                newName: "IdFederacion");

            migrationBuilder.RenameIndex(
                name: "IX_Entrenadores_FederacionId",
                schema: "federacion",
                table: "Entrenadores",
                newName: "IX_Entrenadores_IdFederacion");

            migrationBuilder.RenameColumn(
                name: "FederacionId",
                schema: "federacion",
                table: "DelegadosClub",
                newName: "IdFederacion");

            migrationBuilder.RenameIndex(
                name: "IX_DelegadosClub_FederacionId",
                schema: "federacion",
                table: "DelegadosClub",
                newName: "IX_DelegadosClub_IdFederacion");

            migrationBuilder.RenameColumn(
                name: "FederacionId",
                schema: "catalogos",
                table: "Clubes",
                newName: "ParentClubId");

            migrationBuilder.RenameIndex(
                name: "IX_Clubes_FederacionId",
                schema: "catalogos",
                table: "Clubes",
                newName: "IX_Clubes_ParentClubId");

            migrationBuilder.RenameColumn(
                name: "FederacionId",
                schema: "federacion",
                table: "AtletasFederados",
                newName: "IdFederacion");

            migrationBuilder.RenameIndex(
                name: "IX_AtletasFederados_FederacionId",
                schema: "federacion",
                table: "AtletasFederados",
                newName: "IX_AtletasFederados_IdFederacion");

            migrationBuilder.AddForeignKey(
                name: "FK_AtletasFederados_Federaciones_IdFederacion",
                schema: "federacion",
                table: "AtletasFederados",
                column: "IdFederacion",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "IdFederacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Clubes_Clubes_ParentClubId",
                schema: "catalogos",
                table: "Clubes",
                column: "ParentClubId",
                principalSchema: "catalogos",
                principalTable: "Clubes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DelegadosClub_Federaciones_IdFederacion",
                schema: "federacion",
                table: "DelegadosClub",
                column: "IdFederacion",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "IdFederacion");

            migrationBuilder.AddForeignKey(
                name: "FK_Entrenadores_Federaciones_IdFederacion",
                schema: "federacion",
                table: "Entrenadores",
                column: "IdFederacion",
                principalSchema: "federacion",
                principalTable: "Federaciones",
                principalColumn: "IdFederacion");
        }
    }
}
