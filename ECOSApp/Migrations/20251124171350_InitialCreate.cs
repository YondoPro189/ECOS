using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECOSApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    FotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jueces",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Especialidad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FotoUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jueces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EquipoId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipoNombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    JuezId = table.Column<int>(type: "INTEGER", nullable: false),
                    JuezNombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Puntuacion = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaVoto = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votaciones", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Equipos",
                columns: new[] { "Id", "Descripcion", "FechaRegistro", "FotoUrl", "Nombre" },
                values: new object[,]
                {
                    { 1, "Sistema de votación digital para eventos educativos con tecnología de vanguardia.", new DateTime(2025, 11, 24, 11, 13, 49, 133, DateTimeKind.Local).AddTicks(6859), null, "ECOS-VOTE Innovación" },
                    { 2, "Plataforma de gestión empresarial con inteligencia artificial integrada.", new DateTime(2025, 11, 24, 11, 13, 49, 133, DateTimeKind.Local).AddTicks(7330), null, "Tech Innovators" },
                    { 3, "Aplicación móvil para el seguimiento de proyectos colaborativos en tiempo real.", new DateTime(2025, 11, 24, 11, 13, 49, 133, DateTimeKind.Local).AddTicks(7332), null, "Code Masters" }
                });

            migrationBuilder.InsertData(
                table: "Jueces",
                columns: new[] { "Id", "Descripcion", "Especialidad", "FechaRegistro", "FotoUrl", "Nombre" },
                values: new object[,]
                {
                    { 1, "Especialista en desarrollo de software y metodologías ágiles con 15 años de experiencia.", "Tecnología e Innovación", new DateTime(2025, 11, 24, 11, 13, 49, 134, DateTimeKind.Local).AddTicks(7940), null, "Dr. Carlos Méndez" },
                    { 2, "Experta en experiencia de usuario y diseño de interfaces digitales centradas en el usuario.", "Diseño UX/UI", new DateTime(2025, 11, 24, 11, 13, 49, 134, DateTimeKind.Local).AddTicks(8483), null, "Dra. Ana García" },
                    { 3, "Arquitecto de soluciones en la nube con amplia experiencia en sistemas escalables.", "Arquitectura de Software", new DateTime(2025, 11, 24, 11, 13, 49, 134, DateTimeKind.Local).AddTicks(8486), null, "Ing. Roberto Sánchez" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votaciones_EquipoId",
                table: "Votaciones",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Votaciones_JuezId",
                table: "Votaciones",
                column: "JuezId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Jueces");

            migrationBuilder.DropTable(
                name: "Votaciones");
        }
    }
}
