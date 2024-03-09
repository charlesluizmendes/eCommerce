using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infrastructure.Migrations
{
    public partial class addCardPix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CardId",
                table: "Payment",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PixId",
                table: "Payment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Card",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateValidate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecurityCode = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Card", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pix",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pix", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payment_CardId",
                table: "Payment",
                column: "CardId",
                unique: true,
                filter: "[CardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_PixId",
                table: "Payment",
                column: "PixId",
                unique: true,
                filter: "[PixId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Card_CardId",
                table: "Payment",
                column: "CardId",
                principalTable: "Card",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Pix_PixId",
                table: "Payment",
                column: "PixId",
                principalTable: "Pix",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Card_CardId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Pix_PixId",
                table: "Payment");

            migrationBuilder.DropTable(
                name: "Card");

            migrationBuilder.DropTable(
                name: "Pix");

            migrationBuilder.DropIndex(
                name: "IX_Payment_CardId",
                table: "Payment");

            migrationBuilder.DropIndex(
                name: "IX_Payment_PixId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "CardId",
                table: "Payment");

            migrationBuilder.DropColumn(
                name: "PixId",
                table: "Payment");
        }
    }
}
