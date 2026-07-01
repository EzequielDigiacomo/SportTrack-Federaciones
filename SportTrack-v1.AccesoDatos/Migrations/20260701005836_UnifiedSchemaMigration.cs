using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SportTrack_v1.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedSchemaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "federacion");

            migrationBuilder.AddColumn<int>(
                name: "ParticipanteId",
                schema: "seguridad",
                table: "Usuarios",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentacionPersonas",
                schema: "federacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonaId = table.Column<int>(type: "integer", nullable: true),
                    TipoDocumento = table.Column<int>(type: "integer", nullable: true),
                    UrlArchivo = table.Column<string>(type: "text", nullable: false),
                    PublicId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentacionPersonas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentacionPersonas_Participantes_PersonaId",
                        column: x => x.PersonaId,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Federaciones",
                schema: "federacion",
                columns: table => new
                {
                    IdFederacion = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Cuit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BancoNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoCuenta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    NumeroCuenta = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TitularCuenta = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EmailCobro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Federaciones", x => x.IdFederacion);
                });

            migrationBuilder.CreateTable(
                name: "PagosTransacciones",
                schema: "federacion",
                columns: table => new
                {
                    IdPago = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Concepto = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Monto = table.Column<decimal>(type: "numeric", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IdParticipante = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: false),
                    IdMercadoPago = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosTransacciones", x => x.IdPago);
                    table.ForeignKey(
                        name: "FK_PagosTransacciones_Clubes_IdClub",
                        column: x => x.IdClub,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PagosTransacciones_Participantes_IdParticipante",
                        column: x => x.IdParticipante,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "federacion",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                schema: "federacion",
                columns: table => new
                {
                    IdParticipante = table.Column<int>(type: "integer", nullable: false),
                    TipoTutor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.IdParticipante);
                    table.ForeignKey(
                        name: "FK_Tutores_Participantes_IdParticipante",
                        column: x => x.IdParticipante,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtletasFederados",
                schema: "federacion",
                columns: table => new
                {
                    IdParticipante = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: true),
                    IdFederacion = table.Column<int>(type: "integer", nullable: true),
                    EstadoPago = table.Column<int>(type: "integer", nullable: false),
                    PerteneceSeleccion = table.Column<bool>(type: "boolean", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BecadoEnard = table.Column<bool>(type: "boolean", nullable: false),
                    BecadoSdn = table.Column<bool>(type: "boolean", nullable: false),
                    MontoBeca = table.Column<decimal>(type: "numeric", nullable: false),
                    PresentoAptoMedico = table.Column<bool>(type: "boolean", nullable: false),
                    FechaAptoMedico = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtletasFederados", x => x.IdParticipante);
                    table.ForeignKey(
                        name: "FK_AtletasFederados_Clubes_IdClub",
                        column: x => x.IdClub,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AtletasFederados_Federaciones_IdFederacion",
                        column: x => x.IdFederacion,
                        principalSchema: "federacion",
                        principalTable: "Federaciones",
                        principalColumn: "IdFederacion");
                    table.ForeignKey(
                        name: "FK_AtletasFederados_Participantes_IdParticipante",
                        column: x => x.IdParticipante,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entrenadores",
                schema: "federacion",
                columns: table => new
                {
                    IdParticipante = table.Column<int>(type: "integer", nullable: false),
                    IdClub = table.Column<int>(type: "integer", nullable: true),
                    IdFederacion = table.Column<int>(type: "integer", nullable: true),
                    PerteneceSeleccion = table.Column<bool>(type: "boolean", maxLength: 50, nullable: true),
                    CategoriaSeleccion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BecadoEnard = table.Column<bool>(type: "boolean", nullable: true),
                    BecadoSdn = table.Column<bool>(type: "boolean", nullable: true),
                    MontoBeca = table.Column<decimal>(type: "numeric", nullable: true),
                    PresentoAptoMedico = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entrenadores", x => x.IdParticipante);
                    table.ForeignKey(
                        name: "FK_Entrenadores_Clubes_IdClub",
                        column: x => x.IdClub,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Entrenadores_Federaciones_IdFederacion",
                        column: x => x.IdFederacion,
                        principalSchema: "federacion",
                        principalTable: "Federaciones",
                        principalColumn: "IdFederacion");
                    table.ForeignKey(
                        name: "FK_Entrenadores_Participantes_IdParticipante",
                        column: x => x.IdParticipante,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DelegadosClub",
                schema: "federacion",
                columns: table => new
                {
                    IdParticipante = table.Column<int>(type: "integer", nullable: false),
                    IdRol = table.Column<int>(type: "integer", nullable: false),
                    IdFederacion = table.Column<int>(type: "integer", nullable: true),
                    ClubIdClub = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegadosClub", x => x.IdParticipante);
                    table.ForeignKey(
                        name: "FK_DelegadosClub_Clubes_ClubIdClub",
                        column: x => x.ClubIdClub,
                        principalSchema: "catalogos",
                        principalTable: "Clubes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DelegadosClub_Federaciones_IdFederacion",
                        column: x => x.IdFederacion,
                        principalSchema: "federacion",
                        principalTable: "Federaciones",
                        principalColumn: "IdFederacion");
                    table.ForeignKey(
                        name: "FK_DelegadosClub_Participantes_IdParticipante",
                        column: x => x.IdParticipante,
                        principalSchema: "regatas",
                        principalTable: "Participantes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DelegadosClub_Roles_IdRol",
                        column: x => x.IdRol,
                        principalSchema: "federacion",
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AtletasTutores",
                schema: "federacion",
                columns: table => new
                {
                    IdAtleta = table.Column<int>(type: "integer", nullable: false),
                    IdTutor = table.Column<int>(type: "integer", nullable: false),
                    Parentesco = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtletasTutores", x => new { x.IdAtleta, x.IdTutor });
                    table.ForeignKey(
                        name: "FK_AtletasTutores_AtletasFederados_IdAtleta",
                        column: x => x.IdAtleta,
                        principalSchema: "federacion",
                        principalTable: "AtletasFederados",
                        principalColumn: "IdParticipante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AtletasTutores_Tutores_IdTutor",
                        column: x => x.IdTutor,
                        principalSchema: "federacion",
                        principalTable: "Tutores",
                        principalColumn: "IdParticipante",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "seguridad",
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1,
                column: "ParticipanteId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_ParticipanteId",
                schema: "seguridad",
                table: "Usuarios",
                column: "ParticipanteId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones",
                column: "AtletaFederadoIdParticipante");

            migrationBuilder.CreateIndex(
                name: "IX_AtletasFederados_IdClub",
                schema: "federacion",
                table: "AtletasFederados",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_AtletasFederados_IdFederacion",
                schema: "federacion",
                table: "AtletasFederados",
                column: "IdFederacion");

            migrationBuilder.CreateIndex(
                name: "IX_AtletasTutores_IdTutor",
                schema: "federacion",
                table: "AtletasTutores",
                column: "IdTutor");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadosClub_ClubIdClub",
                schema: "federacion",
                table: "DelegadosClub",
                column: "ClubIdClub");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadosClub_IdFederacion",
                schema: "federacion",
                table: "DelegadosClub",
                column: "IdFederacion");

            migrationBuilder.CreateIndex(
                name: "IX_DelegadosClub_IdRol",
                schema: "federacion",
                table: "DelegadosClub",
                column: "IdRol");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentacionPersonas_PersonaId",
                schema: "federacion",
                table: "DocumentacionPersonas",
                column: "PersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenadores_IdClub",
                schema: "federacion",
                table: "Entrenadores",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_Entrenadores_IdFederacion",
                schema: "federacion",
                table: "Entrenadores",
                column: "IdFederacion");

            migrationBuilder.CreateIndex(
                name: "IX_PagosTransacciones_IdClub",
                schema: "federacion",
                table: "PagosTransacciones",
                column: "IdClub");

            migrationBuilder.CreateIndex(
                name: "IX_PagosTransacciones_IdParticipante",
                schema: "federacion",
                table: "PagosTransacciones",
                column: "IdParticipante");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscripciones_AtletasFederados_AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones",
                column: "AtletaFederadoIdParticipante",
                principalSchema: "federacion",
                principalTable: "AtletasFederados",
                principalColumn: "IdParticipante");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Participantes_ParticipanteId",
                schema: "seguridad",
                table: "Usuarios",
                column: "ParticipanteId",
                principalSchema: "regatas",
                principalTable: "Participantes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscripciones_AtletasFederados_AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Participantes_ParticipanteId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "AtletasTutores",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "DelegadosClub",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "DocumentacionPersonas",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "Entrenadores",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "PagosTransacciones",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "AtletasFederados",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "Tutores",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "federacion");

            migrationBuilder.DropTable(
                name: "Federaciones",
                schema: "federacion");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_ParticipanteId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Inscripciones_AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones");

            migrationBuilder.DropColumn(
                name: "ParticipanteId",
                schema: "seguridad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "AtletaFederadoIdParticipante",
                schema: "regatas",
                table: "Inscripciones");
        }
    }
}
