using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataEF.Migrations
{
    /// <inheritdoc />
    public partial class innit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brackets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Category = table.Column<decimal>(type: "decimal(3,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Brackets__3214EC072D3868F9", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Municipalities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Municipa__3214EC079BF33FDA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ledgers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MunicipalityId = table.Column<int>(type: "int", nullable: false),
                    BracketId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ledgers__3214EC0767890A07", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Brackets_Ledgers",
                        column: x => x.BracketId,
                        principalTable: "Brackets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Municipalities_Ledgers",
                        column: x => x.MunicipalityId,
                        principalTable: "Municipalities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Brackets__3214EC06A41D6B88",
                table: "Brackets",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Brackets__4BB73C3294543B29",
                table: "Brackets",
                column: "Category",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ledgers_BracketId",
                table: "Ledgers",
                column: "BracketId");

            migrationBuilder.CreateIndex(
                name: "IX_Ledgers_MunicipalityId",
                table: "Ledgers",
                column: "MunicipalityId");

            migrationBuilder.CreateIndex(
                name: "UQ__Ledgers__3214EC069CE6108D",
                table: "Ledgers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Municipa__3214EC060EDFBC96",
                table: "Municipalities",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Municipa__737584F66F6049D6",
                table: "Municipalities",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ledgers");

            migrationBuilder.DropTable(
                name: "Brackets");

            migrationBuilder.DropTable(
                name: "Municipalities");
        }
    }
}
