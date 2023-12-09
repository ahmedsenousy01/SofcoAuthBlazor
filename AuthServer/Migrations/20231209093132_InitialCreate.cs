using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientSecret = table.Column<string>(type: "TEXT", nullable: false),
                    RedirectURIs = table.Column<string>(type: "TEXT", nullable: false),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "AccessTokens",
                columns: table => new
                {
                    AccessTokenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccessTokenString = table.Column<string>(type: "TEXT", nullable: true),
                    Scopes = table.Column<string>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessTokens", x => x.AccessTokenId);
                    table.ForeignKey(
                        name: "FK_AccessTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccessTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorizationCodes",
                columns: table => new
                {
                    AuthorizationCodeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AuthCodeString = table.Column<string>(type: "TEXT", nullable: false),
                    CodeChallenge = table.Column<string>(type: "TEXT", nullable: false),
                    CodeChallengeMethod = table.Column<string>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Used = table.Column<bool>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizationCodes", x => x.AuthorizationCodeId);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorizationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    RefreshTokenId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RefreshTokenString = table.Column<string>(type: "TEXT", nullable: false),
                    Expiration = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Revoked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Used = table.Column<bool>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClientId = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorizationCodeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.RefreshTokenId);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AuthorizationCodes_AuthorizationCodeId",
                        column: x => x.AuthorizationCodeId,
                        principalTable: "AuthorizationCodes",
                        principalColumn: "AuthorizationCodeId");
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "EmailConfirmed", "FirstName", "LastName", "PasswordHash", "UserName" },
                values: new object[] { "adminuserid", "admin@sofco.org", false, "John", "Doe", "admin", "admin" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "ClientSecret", "RedirectURIs", "Scopes", "UserId" },
                values: new object[,]
                {
                    { "sofcopayclientid", "2c471168-e53c-4994-848b-ad63a3c2f81fc2816627-b988-4381-9d97-2a73f492f328", "[\"http://localhost:3000\"]", "[\"openid\"]", "adminuserid" },
                    { "sofcosmsclientid", "9f9ec040-4026-4690-8d13-2f31be512fe4dd827a56-f841-468f-b370-6b397ee60ec4", "[\"http://localhost:3001\"]", "[\"openid\"]", "adminuserid" }
                });

            migrationBuilder.InsertData(
                table: "AccessTokens",
                columns: new[] { "AccessTokenId", "AccessTokenString", "ClientId", "Expiration", "Revoked", "Scopes", "UserId" },
                values: new object[,]
                {
                    { 1, null, "sofcopayclientid", new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2580), false, "[]", "adminuserid" },
                    { 2, null, "sofcosmsclientid", new DateTime(2023, 12, 9, 11, 46, 31, 657, DateTimeKind.Local).AddTicks(2585), false, "[]", "adminuserid" }
                });

            migrationBuilder.InsertData(
                table: "AuthorizationCodes",
                columns: new[] { "AuthorizationCodeId", "AuthCodeString", "ClientId", "CodeChallenge", "CodeChallengeMethod", "Expiration", "Used", "UserId" },
                values: new object[,]
                {
                    { 1, "8c12fd67-6134-47c0-937a-cbf84a6f3064", "sofcopayclientid", "authcode1challenge", "Plain", new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2496), false, "adminuserid" },
                    { 2, "6d63ad91-f79c-40df-9810-0dce530eb455", "sofcosmsclientid", "authcode2challenge", "Plain", new DateTime(2023, 12, 9, 11, 41, 31, 657, DateTimeKind.Local).AddTicks(2559), false, "adminuserid" }
                });

            migrationBuilder.InsertData(
                table: "RefreshTokens",
                columns: new[] { "RefreshTokenId", "AuthorizationCodeId", "ClientId", "Expiration", "RefreshTokenString", "Revoked", "Used", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "sofcopayclientid", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "304af016-f88e-4e07-b14b-abb2e3df5143", false, false, "adminuserid" },
                    { 2, 2, "sofcosmsclientid", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "d0439d46-fd2a-4772-9bf4-4801f25825d4", false, false, "adminuserid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_ClientId",
                table: "AccessTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AccessTokens_UserId",
                table: "AccessTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_ClientId",
                table: "AuthorizationCodes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorizationCodes_UserId",
                table: "AuthorizationCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_AuthorizationCodeId",
                table: "RefreshTokens",
                column: "AuthorizationCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ClientId",
                table: "RefreshTokens",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessTokens");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "AuthorizationCodes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
