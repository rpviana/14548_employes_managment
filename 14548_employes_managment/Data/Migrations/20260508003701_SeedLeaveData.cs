using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _14548_employes_managment.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedLeaveData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LeaveTypes",
                columns: new[] { "Id", "Description", "IsActive", "MaxDaysPerYear", "Name" },
                values: new object[,]
                {
                    { 1, "Paid annual leave", true, 30, "Annual Leave" },
                    { 2, "Sick leave", true, 15, "Sick Leave" }
                });

            migrationBuilder.InsertData(
                table: "SystemCodes",
                columns: new[] { "Id", "Code", "Description", "IsActive" },
                values: new object[,]
                {
                    { 1, "LeaveApprovalStatus", "Leave Approval Status", true },
                    { 2, "LeaveDuration", "Leave Duration", true }
                });

            migrationBuilder.InsertData(
                table: "SystemCodeDetails",
                columns: new[] { "Id", "Description", "IsActive", "SystemCodeId" },
                values: new object[,]
                {
                    { 1, "Pending", true, 1 },
                    { 2, "Awaiting Approval", true, 1 },
                    { 3, "Approved", true, 1 },
                    { 4, "Rejected", true, 1 },
                    { 10, "Full Day", true, 2 },
                    { 11, "First Half", true, 2 },
                    { 12, "Second Half", true, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SystemCodeDetails",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SystemCodes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SystemCodes",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
