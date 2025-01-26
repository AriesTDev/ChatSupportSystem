using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatSupportSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastPolledAt",
                table: "ChatSessions");

            migrationBuilder.AddColumn<int>(
                name: "PollCounter",
                table: "ChatSessions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PollCounter",
                table: "ChatSessions");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastPolledAt",
                table: "ChatSessions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
