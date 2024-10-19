using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractorsAuctioneer.Migrations
{
    /// <inheritdoc />
    public partial class addUserEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirsName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "FirsName", "LastName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "79a98b67-0fba-4b24-b433-6fe998264e49", null, null, "AQAAAAIAAYagAAAAEDfAmXi4fkfFXRGn7XJhG+WWwVqCWPtGztdK6bN79ZRAH12+Rd6sGZc6JB3ZAb49Jw==", "43bff35a-c77d-4965-9a1d-8eea9c7575e8" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirsName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5dabb3aa-c99f-4486-bdad-a29709cab95f", "AQAAAAIAAYagAAAAEC6vMIYp2y+q/+RSwkvLg3Zo/cBhzk8GXFIIWLMIjjE4HWQWzgxxJL1E0rF5t//gBw==", "7c70a60e-9b34-489f-a415-490eb5d60fe0" });
        }
    }
}
