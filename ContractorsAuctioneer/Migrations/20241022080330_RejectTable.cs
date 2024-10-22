using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractorsAuctioneer.Migrations
{
    /// <inheritdoc />
    public partial class RejectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reject",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reject", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reject_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b98344a0-7432-4d5e-9f54-09283fe4ddfc", "AQAAAAIAAYagAAAAEKcKDdSMZtPKMVRv2T6t4/6k3P2+nOfsh0zjCur2zpNZoGTj7uzlDGZyVFgJRwKOqw==", "43c17a5a-57a7-4f83-ab6a-6058a48d5086" });

            migrationBuilder.CreateIndex(
                name: "IX_Reject_RequestId",
                table: "Reject",
                column: "RequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reject");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b2994396-612a-4334-9eff-e08b53d6a94b", "AQAAAAIAAYagAAAAEPGY6CjvZHQXZsghii2JQSmbv1mMMKSTQkKhwoc9MSG4DHkO6s17w3xpTclVRiXAgA==", "550b9505-eee4-4284-a9df-ce5d9836be6f" });
        }
    }
}
