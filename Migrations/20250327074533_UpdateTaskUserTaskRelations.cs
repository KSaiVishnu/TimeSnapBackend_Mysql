using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSnapBackend_MySql.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskUserTaskRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<sbyte>(
                name: "Status",
                table: "Tasks",
                type: "TINYINT",
                nullable: false,
                defaultValue: (sbyte)0);
        }
    }
}
