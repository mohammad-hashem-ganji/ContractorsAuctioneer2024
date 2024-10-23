using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractorsAuctioneer.Migrations
{
    /// <inheritdoc />
    public partial class firstName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirsName",
                table: "AspNetUsers",
                newName: "FirstName");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dfde4eb2-0176-40a9-a6bf-8b8bef69fc74", "AQAAAAIAAYagAAAAEPLN8oyXA8MHAZY+BlQAnZpUZb/87es0WW/xT20mnmI/H439kmPohVEKV7KfvNDuoA==", "d4921921-83fd-437b-a8e6-eb6565636f4c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "FirsName");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2a2931ee-b566-4457-99c7-14d33ca5f5aa", "AQAAAAIAAYagAAAAEP14QAcCTeHpCfU6ywyMHXlfySRTRMStkbtgGbRZw7QL361LZbmwzzV9trPkqFGdDg==", "528dfe0f-9562-4303-a654-dae4cd6170bb" });
        }
    }
}
