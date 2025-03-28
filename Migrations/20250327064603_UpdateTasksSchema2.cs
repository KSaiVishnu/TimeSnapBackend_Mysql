using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSnapBackend_MySql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTasksSchema2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_AspNetUsers_AppUserId",
                table: "Timesheets");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_AppUserId",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Timesheets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Timesheets",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_AppUserId",
                table: "Timesheets",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_AspNetUsers_AppUserId",
                table: "Timesheets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
