using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Walkies.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToWalkBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRecords_WalkBooking_WalkBookingId",
                table: "PaymentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_WalkBooking_Users_WalkerId",
                table: "WalkBooking");

            migrationBuilder.DropForeignKey(
                name: "FK_WalkBooking_WalkRequests_WalkRequestId",
                table: "WalkBooking");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalkBooking",
                table: "WalkBooking");

            migrationBuilder.RenameTable(
                name: "WalkBooking",
                newName: "WalkBookings");

            migrationBuilder.RenameIndex(
                name: "IX_WalkBooking_WalkRequestId",
                table: "WalkBookings",
                newName: "IX_WalkBookings_WalkRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_WalkBooking_WalkerId",
                table: "WalkBookings",
                newName: "IX_WalkBookings_WalkerId");

            migrationBuilder.AddColumn<double>(
                name: "CurrentLatitude",
                table: "WalkBookings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CurrentLongitude",
                table: "WalkBookings",
                type: "float",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalkBookings",
                table: "WalkBookings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRecords_WalkBookings_WalkBookingId",
                table: "PaymentRecords",
                column: "WalkBookingId",
                principalTable: "WalkBookings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalkBookings_Users_WalkerId",
                table: "WalkBookings",
                column: "WalkerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalkBookings_WalkRequests_WalkRequestId",
                table: "WalkBookings",
                column: "WalkRequestId",
                principalTable: "WalkRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentRecords_WalkBookings_WalkBookingId",
                table: "PaymentRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_WalkBookings_Users_WalkerId",
                table: "WalkBookings");

            migrationBuilder.DropForeignKey(
                name: "FK_WalkBookings_WalkRequests_WalkRequestId",
                table: "WalkBookings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalkBookings",
                table: "WalkBookings");

            migrationBuilder.DropColumn(
                name: "CurrentLatitude",
                table: "WalkBookings");

            migrationBuilder.DropColumn(
                name: "CurrentLongitude",
                table: "WalkBookings");

            migrationBuilder.RenameTable(
                name: "WalkBookings",
                newName: "WalkBooking");

            migrationBuilder.RenameIndex(
                name: "IX_WalkBookings_WalkRequestId",
                table: "WalkBooking",
                newName: "IX_WalkBooking_WalkRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_WalkBookings_WalkerId",
                table: "WalkBooking",
                newName: "IX_WalkBooking_WalkerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalkBooking",
                table: "WalkBooking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentRecords_WalkBooking_WalkBookingId",
                table: "PaymentRecords",
                column: "WalkBookingId",
                principalTable: "WalkBooking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalkBooking_Users_WalkerId",
                table: "WalkBooking",
                column: "WalkerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalkBooking_WalkRequests_WalkRequestId",
                table: "WalkBooking",
                column: "WalkRequestId",
                principalTable: "WalkRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}