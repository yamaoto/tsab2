using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Rikka.Tsab2.Database.Generator.Migrations
{
    public partial class Initialization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChatId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    StateParams = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SearchEngine",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Additional = table.Column<string>(nullable: true),
                    ChatId = table.Column<int>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchEngine", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkWall",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UploadAlbum = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    VkWallId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkWall", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkAuth",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChatId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ExpiredAt = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkAuth", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkAuth_Chat_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkWallEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    WallId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkWallEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkWallEntry_VkWall_WallId",
                        column: x => x.WallId,
                        principalTable: "VkWall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VkPhoto",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Content = table.Column<byte[]>(type: "image", nullable: true),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    EntryId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    WallId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkPhoto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VkPhoto_VkWallEntry_EntryId",
                        column: x => x.EntryId,
                        principalTable: "VkWallEntry",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VkPhoto_VkWall_WallId",
                        column: x => x.WallId,
                        principalTable: "VkWall",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VkAuth_ChatId",
                table: "VkAuth",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_VkPhoto_EntryId",
                table: "VkPhoto",
                column: "EntryId");

            migrationBuilder.CreateIndex(
                name: "IX_VkPhoto_WallId",
                table: "VkPhoto",
                column: "WallId");

            migrationBuilder.CreateIndex(
                name: "IX_VkWallEntry_WallId",
                table: "VkWallEntry",
                column: "WallId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SearchEngine");

            migrationBuilder.DropTable(
                name: "VkAuth");

            migrationBuilder.DropTable(
                name: "VkPhoto");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "VkWallEntry");

            migrationBuilder.DropTable(
                name: "VkWall");
        }
    }
}
