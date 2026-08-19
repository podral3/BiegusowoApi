using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BiegusowoApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserMakeVoivodeshipNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Voivodeships_VoivodeshipId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "VoivodeshipId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Voivodeships_VoivodeshipId",
                table: "Users",
                column: "VoivodeshipId",
                principalTable: "Voivodeships",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Voivodeships_VoivodeshipId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "VoivodeshipId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Voivodeships_VoivodeshipId",
                table: "Users",
                column: "VoivodeshipId",
                principalTable: "Voivodeships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
