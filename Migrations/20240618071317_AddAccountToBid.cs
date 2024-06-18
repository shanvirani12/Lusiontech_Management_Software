using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lusiontech_Management_Software.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountToBid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountId",
                table: "Bids",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AccountId",
                table: "Bids",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Accounts_AccountId",
                table: "Bids",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Accounts_AccountId",
                table: "Bids");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AccountId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Bids");
        }
    }
}
