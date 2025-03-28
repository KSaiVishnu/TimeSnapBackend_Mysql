using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSnapBackend_MySql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTasksSchema1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assignee",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "EmpId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Task",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskID",
                table: "Tasks");

            migrationBuilder.AlterColumn<double>(
                name: "TotalMinutes",
                table: "Timesheets",
                type: "DOUBLE",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Timesheets",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Timesheets",
                type: "DATETIME(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Timesheets",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<sbyte>(
                name: "Status",
                table: "Tasks",
                type: "TINYINT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "DATETIME(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "DATETIME(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.UpdateData(
                table: "Tasks",
                keyColumn: "BillingType",
                keyValue: null,
                column: "BillingType",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "BillingType",
                table: "Tasks",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TaskName",
                table: "Tasks",
                type: "VARCHAR(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    EmpId = table.Column<string>(type: "VARCHAR(50)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<sbyte>(type: "TINYINT", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "DATETIME(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasks_AspNetUsers_EmpId",
                        column: x => x.EmpId,
                        principalTable: "AspNetUsers",
                        principalColumn: "EmpId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Timesheets_AppUserId",
                table: "Timesheets",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_EmpId",
                table: "UserTasks",
                column: "EmpId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheets_AspNetUsers_AppUserId",
                table: "Timesheets",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timesheets_AspNetUsers_AppUserId",
                table: "Timesheets");

            migrationBuilder.DropTable(
                name: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_Timesheets_AppUserId",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Timesheets");

            migrationBuilder.DropColumn(
                name: "TaskName",
                table: "Tasks");

            migrationBuilder.AlterColumn<double>(
                name: "TotalMinutes",
                table: "Timesheets",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "DOUBLE");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Timesheets",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Timesheets",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Tasks",
                type: "int",
                nullable: false,
                oldClrType: typeof(sbyte),
                oldType: "TINYINT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DueDate",
                table: "Tasks",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "DATETIME(6)");

            migrationBuilder.AlterColumn<string>(
                name: "BillingType",
                table: "Tasks",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Assignee",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "EmpId",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Task",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "TaskID",
                table: "Tasks",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
