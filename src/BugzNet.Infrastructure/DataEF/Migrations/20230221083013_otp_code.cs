using Microsoft.EntityFrameworkCore.Migrations;

namespace BugzNet.Infrastructure.Migrations
{
    public partial class otp_code : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "OTP",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "OTP");
        }
    }
}
