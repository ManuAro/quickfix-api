using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickFixApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateAvailabilitiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "Appointments",
                newName: "time");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Appointments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Appointments",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Appointments",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Appointments",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "ProviderProfession",
                table: "Appointments",
                newName: "provider_profession");

            migrationBuilder.RenameColumn(
                name: "ProviderName",
                table: "Appointments",
                newName: "provider_name");

            migrationBuilder.RenameColumn(
                name: "ProviderId",
                table: "Appointments",
                newName: "provider_id");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Appointments",
                newName: "end_time");

            migrationBuilder.RenameColumn(
                name: "ClientName",
                table: "Appointments",
                newName: "client_name");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Appointments",
                newName: "client_id");

            migrationBuilder.RenameColumn(
                name: "AcceptedByProvider",
                table: "Appointments",
                newName: "accepted_by_provider");

            migrationBuilder.AlterColumn<string>(
                name: "date",
                table: "Appointments",
                type: "text",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "price",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "service_description",
                table: "Appointments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "price",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "service_description",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "time",
                table: "Appointments",
                newName: "Time");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Appointments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Appointments",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Appointments",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "Appointments",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "provider_profession",
                table: "Appointments",
                newName: "ProviderProfession");

            migrationBuilder.RenameColumn(
                name: "provider_name",
                table: "Appointments",
                newName: "ProviderName");

            migrationBuilder.RenameColumn(
                name: "provider_id",
                table: "Appointments",
                newName: "ProviderId");

            migrationBuilder.RenameColumn(
                name: "end_time",
                table: "Appointments",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "client_name",
                table: "Appointments",
                newName: "ClientName");

            migrationBuilder.RenameColumn(
                name: "client_id",
                table: "Appointments",
                newName: "ClientId");

            migrationBuilder.RenameColumn(
                name: "accepted_by_provider",
                table: "Appointments",
                newName: "AcceptedByProvider");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
