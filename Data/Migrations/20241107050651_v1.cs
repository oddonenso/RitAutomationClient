using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Equipment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeID",
                table: "Equipment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    statusName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Type",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    typeName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Type", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_StatusID",
                table: "Equipment",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Equipment_TypeID",
                table: "Equipment",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Status_StatusID",
                table: "Equipment",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipment_Type_TypeID",
                table: "Equipment",
                column: "TypeID",
                principalTable: "Type",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Status_StatusID",
                table: "Equipment");

            migrationBuilder.DropForeignKey(
                name: "FK_Equipment_Type_TypeID",
                table: "Equipment");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Type");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_StatusID",
                table: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Equipment_TypeID",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Equipment");

            migrationBuilder.DropColumn(
                name: "TypeID",
                table: "Equipment");
        }
    }
}
