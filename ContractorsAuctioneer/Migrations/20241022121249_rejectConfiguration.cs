using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractorsAuctioneer.Migrations
{
    /// <inheritdoc />
    public partial class rejectConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reject_Requests_RequestId",
                table: "Reject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reject",
                table: "Reject");

            migrationBuilder.RenameTable(
                name: "Reject",
                newName: "Rejects");

            migrationBuilder.RenameIndex(
                name: "IX_Reject_RequestId",
                table: "Rejects",
                newName: "IX_Rejects_RequestId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Rejects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Rejects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Rejects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "Rejects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Rejects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Rejects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Rejects",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rejects",
                table: "Rejects",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2a2931ee-b566-4457-99c7-14d33ca5f5aa", "AQAAAAIAAYagAAAAEP14QAcCTeHpCfU6ywyMHXlfySRTRMStkbtgGbRZw7QL361LZbmwzzV9trPkqFGdDg==", "528dfe0f-9562-4303-a654-dae4cd6170bb" });

            migrationBuilder.AddForeignKey(
                name: "FK_Rejects_Requests_RequestId",
                table: "Rejects",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rejects_Requests_RequestId",
                table: "Rejects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rejects",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Rejects");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Rejects");

            migrationBuilder.RenameTable(
                name: "Rejects",
                newName: "Reject");

            migrationBuilder.RenameIndex(
                name: "IX_Rejects_RequestId",
                table: "Reject",
                newName: "IX_Reject_RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reject",
                table: "Reject",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b98344a0-7432-4d5e-9f54-09283fe4ddfc", "AQAAAAIAAYagAAAAEKcKDdSMZtPKMVRv2T6t4/6k3P2+nOfsh0zjCur2zpNZoGTj7uzlDGZyVFgJRwKOqw==", "43c17a5a-57a7-4f83-ab6a-6058a48d5086" });

            migrationBuilder.AddForeignKey(
                name: "FK_Reject_Requests_RequestId",
                table: "Reject",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
