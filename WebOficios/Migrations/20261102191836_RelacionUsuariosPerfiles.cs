using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oficios.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RelacionUsuariosPerfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Jobs_JobId1",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Workers_JobId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Clients_ClientId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Workers_ClientId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Payments_JobId1",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "JobId1",
                table: "Payments");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Workers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Workers_UserId",
                table: "Workers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_User_UserId",
                table: "Clients",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Jobs_JobId",
                table: "Payments",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_User_UserId",
                table: "Workers",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_User_UserId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Jobs_JobId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_User_UserId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Workers_UserId",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Clients_UserId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Workers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Clients");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Workers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobId1",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Workers_ClientId",
                table: "Workers",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_JobId1",
                table: "Payments",
                column: "JobId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Jobs_JobId1",
                table: "Payments",
                column: "JobId1",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Workers_JobId",
                table: "Payments",
                column: "JobId",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Clients_ClientId",
                table: "Workers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
