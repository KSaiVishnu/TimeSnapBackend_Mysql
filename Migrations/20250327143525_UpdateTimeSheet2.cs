using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSnapBackend_MySql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTimeSheet2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets");

            migrationBuilder.AlterColumn<string>(
                name: "TaskId",
                table: "Timesheets",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "TaskId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "Timesheets",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_Tasks_TaskId",
                table: "Timesheets",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
