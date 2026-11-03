using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficios.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AgregarRespuestaClienteAProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientResponse",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "Proposals",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientResponse",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "Proposals");
        }
    }
}
