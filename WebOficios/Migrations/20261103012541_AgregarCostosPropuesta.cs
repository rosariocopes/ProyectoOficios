using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficios.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCostosPropuesta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedMaterialsCost",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LaborCost",
                table: "Proposals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedMaterialsCost",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "LaborCost",
                table: "Proposals");
        }
    }
}
