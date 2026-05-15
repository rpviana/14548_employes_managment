using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _14548_employes_managment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeFunction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Function",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Function",
                table: "Employees");
        }
    }
}
