using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficios.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProposalId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ProposalId",
                table: "Payments",
                column: "ProposalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Proposals_ProposalId",
                table: "Payments",
                column: "ProposalId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Proposals_ProposalId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ProposalId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "Payments");
        }
    }
}
