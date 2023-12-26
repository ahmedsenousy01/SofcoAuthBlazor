using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthServer.Migrations
{
    /// <inheritdoc />
    public partial class AddedNameFieldForClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Clients",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AccessTokens",
                keyColumn: "AccessTokenId",
                keyValue: 1,
                column: "Expiration",
                value: new DateTime(2023, 12, 26, 12, 8, 48, 405, DateTimeKind.Local).AddTicks(9178));

            migrationBuilder.UpdateData(
                table: "AccessTokens",
                keyColumn: "AccessTokenId",
                keyValue: 2,
                column: "Expiration",
                value: new DateTime(2023, 12, 26, 12, 8, 48, 405, DateTimeKind.Local).AddTicks(9185));

            migrationBuilder.UpdateData(
                table: "AuthorizationCodes",
                keyColumn: "AuthorizationCodeId",
                keyValue: 1,
                columns: new[] { "AuthCodeString", "Expiration" },
                values: new object[] { "4448e663-f412-4bc5-bab7-31b8c33a394d", new DateTime(2023, 12, 26, 12, 3, 48, 405, DateTimeKind.Local).AddTicks(9061) });

            migrationBuilder.UpdateData(
                table: "AuthorizationCodes",
                keyColumn: "AuthorizationCodeId",
                keyValue: 2,
                columns: new[] { "AuthCodeString", "Expiration" },
                values: new object[] { "383f5e1f-7fbe-49e4-b53d-bd4b77a4b660", new DateTime(2023, 12, 26, 12, 3, 48, 405, DateTimeKind.Local).AddTicks(9139) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: "sofcopayclientid",
                columns: new[] { "ClientSecret", "Name" },
                values: new object[] { "5ad092f4-94e9-469c-aa7b-d9c077d5dea7672bf35d-c928-4c03-9735-1b612230cd81", "sofco pay" });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: "sofcosmsclientid",
                columns: new[] { "ClientSecret", "Name" },
                values: new object[] { "8ae3f553-e391-4614-9d0f-a796ba630da2109f83a3-b36a-486b-b394-8a07f11beecb", "sofco sms" });

            migrationBuilder.UpdateData(
                table: "RefreshTokens",
                keyColumn: "RefreshTokenId",
                keyValue: 1,
                column: "RefreshTokenString",
                value: "b156a22d-6406-4454-b37d-15ba7d7a01de");

            migrationBuilder.UpdateData(
                table: "RefreshTokens",
                keyColumn: "RefreshTokenId",
                keyValue: 2,
                column: "RefreshTokenString",
                value: "d777e4af-fd94-402a-b423-8efbe504c3e5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Clients");

            migrationBuilder.UpdateData(
                table: "AccessTokens",
                keyColumn: "AccessTokenId",
                keyValue: 1,
                column: "Expiration",
                value: new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2580));

            migrationBuilder.UpdateData(
                table: "AccessTokens",
                keyColumn: "AccessTokenId",
                keyValue: 2,
                column: "Expiration",
                value: new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2585));

            migrationBuilder.UpdateData(
                table: "AuthorizationCodes",
                keyColumn: "AuthorizationCodeId",
                keyValue: 1,
                columns: new[] { "AuthCodeString", "Expiration" },
                values: new object[] { "8c12fd67-6134-47c0-937a-cbf84a6f3064", new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2496) });

            migrationBuilder.UpdateData(
                table: "AuthorizationCodes",
                keyColumn: "AuthorizationCodeId",
                keyValue: 2,
                columns: new[] { "AuthCodeString", "Expiration" },
                values: new object[] { "6d63ad91-f79c-40df-9810-0dce530eb455", new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2559) });

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: "sofcopayclientid",
                column: "ClientSecret",
                value: "2c471168-e53c-4994-848b-ad63a3c2f81fc2816627-b988-4381-9d97-2a73f492f328");

            migrationBuilder.UpdateData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: "sofcosmsclientid",
                column: "ClientSecret",
                value: "9f9ec040-4026-4690-8d13-2f31be512fe4dd827a56-f841-468f-b370-6b397ee60ec4");

            migrationBuilder.UpdateData(
                table: "RefreshTokens",
                keyColumn: "RefreshTokenId",
                keyValue: 1,
                column: "RefreshTokenString",
                value: "304af016-f88e-4e07-b14b-abb2e3df5143");

            migrationBuilder.UpdateData(
                table: "RefreshTokens",
                keyColumn: "RefreshTokenId",
                keyValue: 2,
                column: "RefreshTokenString",
                value: "d0439d46-fd2a-4772-9bf4-4801f25825d4");
        }
    }
}
