using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactsManager.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_People_TaxIdentificationNumber",
                table: "People");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "People");

            migrationBuilder.DropColumn(
                name: "ReceiveNewsLetters",
                table: "People");

            migrationBuilder.DropColumn(
                name: "TaxIdentificationNumber",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "People",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "People");

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "People",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "ReceiveNewsLetters",
                table: "People",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TaxIdentificationNumber",
                table: "People",
                type: "varchar(8)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_People_TaxIdentificationNumber",
                table: "People",
                column: "TaxIdentificationNumber",
                unique: true);
        }
    }
}
