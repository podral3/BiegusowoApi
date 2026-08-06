using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiegusowoApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Blobassetconnection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssetId",
                table: "Blobs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "AssetType",
                table: "Blobs",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssetId",
                table: "Blobs");

            migrationBuilder.DropColumn(
                name: "AssetType",
                table: "Blobs");
        }
    }
}
